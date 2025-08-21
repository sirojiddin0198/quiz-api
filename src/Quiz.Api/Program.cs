using Quiz.CSharp.Api.Extensions;
using Quiz.CSharp.Data.Extensions;
using Quiz.Infrastructure.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Quiz.CSharp.Data.Data;
using Quiz.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureAppConfiguration(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddSwaggerWithOAuth(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCSharpData(builder.Configuration);
builder.Services.AddCSharpApi();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<CSharpDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseSwaggerWithOAuth();

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
