using Api.Configuration;
using Api.ErrorHandler;
using Business.Jwt;
using Hangfire;
using Hangfire.PostgreSql;
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
    });

    // Firebase
    builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("Firebase"));

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