using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
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

    builder.Services.AddControllers().AddNewtonsoftJson();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.ConfigureServices(builder.Configuration, builder.Environment.IsDevelopment());

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Bearer"; // Schéma par défaut pour l'API
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Google pour le challenge
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["GoogleToken:Id"]; // ID Client Google
        googleOptions.ClientSecret = builder.Configuration["GoogleToken:Secret"]; // Secret Client Google
        googleOptions.CallbackPath = "/signin-google"; // URI de redirection
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com"; // Autorité de Google
        options.Audience = "VOTRE_CLIENT_ID"; // ID Client Google pour valider le token
    });

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