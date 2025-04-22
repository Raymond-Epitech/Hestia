using Api.ErrorHandler;
using Business.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApi.Configuration;

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
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Bearer"; // Schéma par défaut pour l'API
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
        
        if (jwtOptions is null || jwtOptions.Issuer is null || jwtOptions.Audience is null)
        {
            throw new Exception("Missing jwt options");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, //Set to true to check expire
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Token JWT manquant ou invalide.\"}");
            }
        };
    });

    // Authorization
    builder.Services.AddAuthorization();

    // Swagger
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Saisissez votre token JWT.\n\nExemple : 'abc123def456ghi'"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    var app = builder.Build();

    app.UseCors();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseExceptionHandler();
    app.UseStatusCodePages(); 
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    return 1;
}

return 0;