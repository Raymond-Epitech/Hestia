using BookStoreApi.Services;
using Business.Interfaces;
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
        }

        private static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IReminderService, ReminderService>();
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
            services.AddDbContext<HestiaContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("ConnectionString"));
                opt.EnableDetailedErrors(isDevelopment);
                opt.EnableSensitiveDataLogging(isDevelopment);
            });

            return services;
        }
    }
}
