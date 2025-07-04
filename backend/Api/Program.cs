using Api.Configuration;
using Api.ErrorHandler;
using Business.Jwt;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Mvc;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseDefaultServiceProvider(o =>
    {
        o.ValidateOnBuild = true;
        o.ValidateScopes = true;
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

    // Hangfire
    builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HestiaDb")))
    );
    builder.Services.AddHangfireServer();

    var app = builder.Build();

    app.UseCors();

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

    app.UseExceptionHandler();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

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