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

    // Dans Program.cs, juste après builder.Services.AddControllers();

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // 1. Récupérer toutes les erreurs de validation
            var errors = context.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Message = e.Value!.Errors.First().ErrorMessage
                })
                .ToList();

            // 2. Construire le message "Detail" pour ressembler à ton ExceptionHandler
            // On concatène les erreurs pour n'avoir qu'une string dans "Detail"
            var errorMessages = string.Join("; ", errors.Select(e => $"{e.Field}: {e.Message}"));

            // 3. Créer le ProblemDetails uniformisé
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Invalid input data", // Exactement le même titre que ton ExceptionHandler
                Detail = errorMessages,       // On met les erreurs ici au lieu d'un objet "errors"
                Instance = context.HttpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc4918#section-11.2" // Uniformité du Type
            };

            // 4. Renvoyer le résultat
            return new UnprocessableEntityObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

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

    // Health checks
    builder.Services.AddHealthChecks();
    builder.Services.AddSignalR(o =>
    {
        o.EnableDetailedErrors = true;
    });

    // Hangfire
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("HestiaDb")))
        );
    builder.Services.AddHangfireServer(); // OK, à conserver
    builder.Logging.SetMinimumLevel(LogLevel.Debug);

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
        app.MapHealthChecks("/health");
        app.UseHttpsRedirection();
    }

    //app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapHub<HestiaHub>("/hestiaHub");

    // Configure Hangfire recurring jobs
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        using var scope = app.Services.CreateScope();
        var cfg = scope.ServiceProvider.GetRequiredService<RecurringJobsConfigurator>();
        cfg.Configure();
    });

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    return 1;
}

return 0;