using Business.Interfaces;
using Business.Services;
using EntityFramework.Context;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Configuration;

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
        services.AddScoped<IShoppingListService, ShoppingListService>();
        services.AddScoped<IExpiredChoreRemover, ExpiredChoreRemover>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IRealTimeService, RealTimeService>();
        services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IPollService, PollService>();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Others
        services.AddHttpContextAccessor();
        services.AddScoped<RecurringJobsConfigurator>();
        return services;
    }

    public static IServiceCollection EnableCors(this IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy("AllowFrontend", policy => policy
                .WithOrigins("http://localhost:3000", "https://hestiaapp.org")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
        });

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

