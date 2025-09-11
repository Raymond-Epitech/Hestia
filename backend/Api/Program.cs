using Api.Configuration;
using Api.ErrorHandler;
using Business.Jwt;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Configuration;
using SignalR.Hubs;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseDefaultServiceProvider(o =>
    {
        o.ValidateOnBuild = true;
        o.ValidateScopes = true;
    });

    // Kestrel
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8081);
    });

    builder.Configuration["BasicAuth:Username"] ??= "admin";
    builder.Configuration["BasicAuth:Password"] ??= "ChangeMe!Tr3sLong";

    // Controllers
    builder.Services.AddControllers().AddNewtonsoftJson();
    builder.Services.AddEndpointsApiExplorer();

    // ProblemDetails + ExceptionHandler
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<ExceptionHandler>();

    // Divers
    builder.Services.AddLazyCache();
    builder.Services.AddHttpClient();

    // Validation 422 personnalisée
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Validation error",
                Instance = context.HttpContext.Request.Path
            };
            return new UnprocessableEntityObjectResult(problemDetails);
        };
    });

    builder.Services.ConfigureServices(builder.Configuration, builder.Environment.IsDevelopment());
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
    builder.Services.ConfigureJWT(builder.Configuration, builder.Environment.IsDevelopment());
    builder.Services.ConfigureSwagger(builder.Configuration, builder.Environment.IsDevelopment());

    builder.Services.AddAuthentication()
        .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", _ => { });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("BasicOnly",
            p => p.AddAuthenticationSchemes("Basic").RequireAuthenticatedUser());
    });

    // SignalR
    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(45);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.MaximumReceiveMessageSize = 64 * 1024; // 64 KB
    });

    // HealthChecks + UI (UI sonde un endpoint INTERNE loopback)
    builder.Services.AddHealthChecks()
        .AddCheck<SignalRHealthCheck>("signalr_hub");

    builder.Services.AddHealthChecksUI(setup =>
    {
        setup.SetEvaluationTimeInSeconds(15);
        setup.MaximumHistoryEntriesPerEndpoint(60);
        setup.AddHealthCheckEndpoint("api-self", "http://127.0.0.1:8081/_health-internal");
    })
    .AddInMemoryStorage();

    // Logging ciblé pour SignalR
    builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
    builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

    // Firebase
    builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("Firebase"));
    var firebasePrivateKey = builder.Configuration["Firebase:PrivateKey"]?.Replace("\\n", "\n");
    var serviceAccountJson = $@"
    {{
      ""type"": ""service_account"",
      ""project_id"": ""{builder.Configuration["Firebase:ProjectId"]}"",
      ""private_key_id"": ""{builder.Configuration["Firebase:PrivateKeyId"]}"",
      ""private_key"": ""{firebasePrivateKey}"",
      ""client_email"": ""{builder.Configuration["Firebase:ClientEmail"]}"",
      ""client_id"": ""{builder.Configuration["Firebase:ClientId"]}"",
      ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
      ""token_uri"": ""https://oauth2.googleapis.com/token"",
      ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
      ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/{Uri.EscapeDataString(builder.Configuration["Firebase:ClientEmail"]!)}""
    }}";

    GoogleCredential credential = GoogleCredential.FromJson(serviceAccountJson);
    if (FirebaseApp.DefaultInstance == null)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = credential,
            ProjectId = builder.Configuration["Firebase:ProjectId"]
        });
    }
    builder.Services.AddSingleton(provider =>
    {
        var app = FirebaseApp.DefaultInstance;
        return FirebaseAdmin.Messaging.FirebaseMessaging.GetMessaging(app);
    });

    // Hangfire
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>
            options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HestiaDb")))
    );
    builder.Services.AddHangfireServer();

    // Sentry
    builder.WebHost.UseSentry(o =>
    {
        o.Dsn = builder.Configuration["Sentry"];
        o.Debug = true;
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseStatusCodePages();
    }

    app.UseExceptionHandler();

    app.UseRouting();
    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    app.Use(async (ctx, next) =>
    {
        var p = ctx.Request.Path;

        bool needsBasic =
            p.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase) ||
            p.StartsWithSegments("/health-api", StringComparison.OrdinalIgnoreCase) ||
            p.StartsWithSegments("/hangfire", StringComparison.OrdinalIgnoreCase);

        if (!needsBasic)
        {
            await next();
            return;
        }

        var result = await ctx.AuthenticateAsync("Basic");
        if (!result.Succeeded)
        {
            await ctx.ChallengeAsync("Basic");
            return;
        }

        ctx.User = result.Principal!;
        await next();
    });

    // ---- Endpoints ----

    app.MapHealthChecks("/_health-internal", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).RequireHost("127.0.0.1", "localhost");

    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/health";
        options.ApiPath = "/health-api";
    }).RequireAuthorization("BasicOnly");

    app.MapHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new List<IDashboardAuthorizationFilter>()
    }).RequireAuthorization("BasicOnly");

    // WebSockets + SignalR Hub
    app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(15) });
    app.MapHub<HestiaHub>("/hestiaHub");

    // Controllers
    app.MapControllers();

    // Recurring jobs
    using (var scope = app.Services.CreateScope())
    {
        var jobConfigurator = scope.ServiceProvider.GetRequiredService<RecurringJobsConfigurator>();
        jobConfigurator.Configure();
    }

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    return 1;
}

return 0;
