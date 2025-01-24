using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using System.Text;
using WebApi.Configuration;
using Business.Models.Jwt;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseDefaultServiceProvider(o =>
    {
        o.ValidateOnBuild = true;
        o.ValidateScopes = true;
    });

    builder.Services.AddControllers().AddNewtonsoftJson();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.ConfigureServices(builder.Configuration, builder.Environment.IsDevelopment());
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Bearer"; // Schéma par défaut pour l'API
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Google pour le challenge
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    };

    builder.Services.AddAuthorization();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseCors();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        using (var connection = new Npgsql.NpgsqlConnection(builder.Configuration.GetConnectionString("HestiaDb")))
        {
            try
            {
                if (string.IsNullOrEmpty(builder.Configuration.GetConnectionString("HestiaDb")))
                {
                    Console.Error.WriteLine("Connection string is not set in Docker.");
                }
                connection.Open();
                Console.Error.WriteLine("Connection successful!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Connection failed: {ex.Message}");
            }
        }
    }

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    return 1;
}

return 0;