using Business.Interfaces;
using Business.Services;
using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories;
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
            // Services
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IChoreService, ChoreService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IColocationService, ColocationService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IExpenseService, ExpenseService>();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Others
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
                opt.UseNpgsql(configuration.GetConnectionString("HestiaDb"));
                opt.EnableDetailedErrors(isDevelopment);
                opt.EnableSensitiveDataLogging(isDevelopment);
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            return services;
        }
    }
}
