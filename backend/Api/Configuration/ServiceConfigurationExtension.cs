using Business.Interfaces;
using Business.Models.Jwt;
using Business.Services;
using EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Configuration
{
    public static class ServiceConfigurationExtension
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.AddDbContext(configuration, isDevelopment)
                .AddBusinessServices()
                .EnableCors();
            //AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("System.Globalization.Invariant", false);
        }

        private static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IChoreService, ChoreService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICollocationService, CollocationService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddHttpContextAccessor();
            return services;
        }

        public static IServiceCollection EnableCors(this IServiceCollection services)
        {
            var allowedHost = new[]{
                "http://localhost:3000"
            };
            services.AddCors(opt => opt.AddDefaultPolicy(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(host => allowedHost.Contains(host))));
            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            /*services.AddDbContext<HestiaContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("HestiaDb"));
                opt.EnableDetailedErrors(isDevelopment);
                opt.EnableSensitiveDataLogging(isDevelopment);
            });*/

            services.AddDbContext<HestiaContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("HestiaDb"));

                if (isDevelopment) // Vérifie si on est en mode développement
                {
                    opt.EnableDetailedErrors(); // Active les erreurs détaillées en développement
                    opt.EnableSensitiveDataLogging(); // Active les logs de données sensibles en développement
                }
            });

            return services;
        }
    }
}
