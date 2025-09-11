using Api.Configuration;
using Api.ErrorHandler;
using Business.Jwt;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
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

    //var cert = X509Certificate2.CreateFromPemFile("/etc/ssl/certificate.pem", "/etc/ssl/key.pem");

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8081);

        /*options.ListenAnyIP(8080, listenOptions =>
        {
            listenOptions.UseHttps(cert);
        });*/
    });

    // Controllers
    builder.Services.AddMvcCore();
    builder.Services.AddControllers().AddNewtonsoftJson();

    // Error handling
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<ExceptionHandler>();

    builder.Services.AddEndpointsApiExplorer();

    // Ajout de LazyCache
    builder.Services.AddLazyCache();

    // Add HttpClient
    builder.Services.AddHttpClient();

    // Services and DI
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

    // Authentication
    builder.Services.ConfigureJWT(builder.Configuration, builder.Environment.IsDevelopment());

    // Authorization
    builder.Services.AddAuthorization();

    // Swagger
    builder.Services.ConfigureSwagger(builder.Configuration, builder.Environment.IsDevelopment());

    // SignalR
    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
        // Si le client reste silencieux plus longtemps que ça, on le considère déconnecté
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(45);
        // Ping serveur => client (doit être inférieur au timeout client)
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        // Taille max (ajuste si tu envoies des gros payloads)
        options.MaximumReceiveMessageSize = 64 * 1024; // 64 KB
    });

    builder.Services.AddHealthChecks()
        .AddCheck<SignalRHealthCheck>("signalr_hub");
    builder.Services.AddHealthChecksUI(setup =>
    {
        setup.SetEvaluationTimeInSeconds(15);
        setup.MaximumHistoryEntriesPerEndpoint(60);
        setup.AddHealthCheckEndpoint("api-self", "http://localhost:8081/healthz");
    }).AddInMemoryStorage();

    builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
    builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

    // Firebase
    builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("Firebase"));
    var projectId = builder.Configuration["Firebase:ProjectId"];
    var serviceAccountJson = $@"
    {{
      ""type"": ""service_account"",
      ""project_id"": ""{builder.Configuration["Firebase:ProjectId"]}"",
      ""private_key_id"": ""{builder.Configuration["Firebase:PrivateKeyId"]}"",
      ""private_key"": ""{builder.Configuration["Firebase:PrivateKey"]}"",
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
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
    builder.Services.AddHangfireServer();

    builder.WebHost.UseSentry(o =>
    {
        o.Dsn = builder.Configuration["Sentry"];
        o.Debug = true;
    });

    var app = builder.Build();

    app.UseRouting();
    app.UseCors("AllowFrontend");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseStatusCodePages();
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [new AllowAllDashboardAuthorization()]
        });
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    //app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.UseWebSockets(new WebSocketOptions
    {
        KeepAliveInterval = TimeSpan.FromSeconds(15)
    });

    app.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/health";
        options.ApiPath = "/health-api";
    });

    app.MapHub<HestiaHub>("/hestiaHub");

    // Configure Hangfire recurring jobs
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