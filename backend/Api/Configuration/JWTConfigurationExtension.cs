using Business.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Configuration;

public static class JWTConfigurationExtension
{
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Bearer";
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

            if (jwtOptions is null || jwtOptions.Issuer is null || jwtOptions.Audience is null)
            {
                throw new Exception("Missing jwt options");
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = !isDevelopment,
                ClockSkew = TimeSpan.FromMinutes(30),
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
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
    }
}
