namespace Quiz.CSharp.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quiz.CSharp.Data.Services;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.CSharp.Data.Repositories;
using Quiz.CSharp.Data.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ICSharpDbContext, CSharpDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("CSharpQuiz"))
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IAnswerRepository, AnswerRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddHostedService<DatabaseMigrationService>();
        services.AddHostedService<DataSeedingService>();

        return services;
    }
} 