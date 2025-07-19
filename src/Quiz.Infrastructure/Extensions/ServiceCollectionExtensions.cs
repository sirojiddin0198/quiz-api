namespace Quiz.Infrastructure.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ApplicationInsights.Extensibility;
using Quiz.Infrastructure.Authentication;
using Quiz.Infrastructure.Telemetry;
using Quiz.Shared.Authentication;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Keycloak:Authority"];
                options.Audience = configuration["Keycloak:Audience"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("Keycloak:RequireHttpsMetadata");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        services.AddApplicationInsightsTelemetry();
        services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>();

        return services;
    }

    public static IServiceCollection AddAzureAppConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        var appConfigConnectionString = configuration["AppConfig:ConnectionString"];
        var copyOfSources = configuration.Sources.ToList();
        if (string.IsNullOrWhiteSpace(appConfigConnectionString) is false)
        {
            configuration.AddAzureAppConfiguration(o =>
            {
                o.Connect(appConfigConnectionString);
                o.Select(KeyFilter.Any, LabelFilter.Null);

                // config label selection.
                var labels = configuration["AppConfig:Labels"]?.Split(',') ?? [];
                foreach (var label in labels)
                    o.Select(KeyFilter.Any, label);
            });

            if (configuration.GetValue("AppConfig:ReOrderSources", false))
            {
                var secretsSource = configuration.Sources
                    .FirstOrDefault(t => t is JsonConfigurationSource jsonConfigurationSource && jsonConfigurationSource.Path?.EndsWith("secrets.json") is true);
                configuration.Sources.Add(secretsSource!);
            }
        }

        return services;
    }
} 