namespace Quiz.CSharp.Api.Extensions;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Quiz.CSharp.Api.Mapping;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.CSharp.Api.Validators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpApi(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CollectionProfile).Assembly);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(SubmitAnswerRequestValidator).Assembly);

        services.AddScoped<IAnswerService, AnswerService>();
        services.AddScoped<IAnswerValidator, AnswerValidator>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IResultsService, ResultsService>();
        services.AddScoped<ISubscriptionGuard, SubscriptionGuard>();
        services.AddScoped<IUserProgressService, UserProgressService>();

        return services;
    }
} 