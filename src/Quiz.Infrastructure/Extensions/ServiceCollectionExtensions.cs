namespace Quiz.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Quiz.Infrastructure.Authentication;
using Quiz.Infrastructure.Telemetry;
using Quiz.Shared.Authentication;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.OpenApi.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Add Keycloak authentication
        services.AddKeycloakWebApiAuthentication(configuration);

        // Add Keycloak authorization with policies
        services.AddAuthorization()
            .AddKeycloakAuthorization(configuration)
            .AddAuthorizationBuilder()
            .AddPolicy("Admin:Read", policy =>
                policy.RequireResourceRoles("admin:read"))
            .AddPolicy("Admin:Write", policy =>
                policy.RequireResourceRoles("admin:write"))
            .AddPolicy("Admin:Manage", policy =>
                policy.RequireResourceRoles("admin:read", "admin:write"));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        services.AddApplicationInsightsTelemetry();
        services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>();

        return services;
    }

    public static IServiceCollection AddSwaggerWithOAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Quiz Platform API", Version = "v1" });
            
            // Add OAuth2/OIDC authentication for Keycloak
            // In Swagger UI, click "Authorize" and use OAuth2 (oauth2) to authenticate with Keycloak
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{configuration["Keycloak:auth-server-url"]}realms/{configuration["Keycloak:realm"]}/protocol/openid-connect/auth"),
                        TokenUrl = new Uri($"{configuration["Keycloak:auth-server-url"]}realms/{configuration["Keycloak:realm"]}/protocol/openid-connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect scope" },
                            { "profile", "Profile information" },
                            { "email", "Email address" }
                        }
                    }
                }
            });

            // Keep Bearer token as fallback
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    new[] { "openid", "profile", "email" }
                }
            });
        });

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