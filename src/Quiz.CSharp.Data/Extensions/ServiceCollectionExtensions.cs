namespace Quiz.CSharp.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quiz.CSharp.Data.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CSharpDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICSharpRepository, CSharpRepository>();

        return services;
    }
} 