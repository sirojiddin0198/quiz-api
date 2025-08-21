namespace Quiz.Infrastructure.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

public static class WebApplicationExtensions
{
    public static WebApplication UseSwaggerWithOAuth(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quiz Platform API v1");
                
                c.OAuthClientId(app.Configuration["Keycloak:resource"]);
                c.OAuthAppName("Quiz Platform API");
                c.OAuthUsePkce();
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
                {
                    { "nonce", Guid.NewGuid().ToString() }
                });
            });
        }

        return app;
    }
} 