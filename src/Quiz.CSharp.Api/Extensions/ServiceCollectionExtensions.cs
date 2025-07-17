namespace Quiz.CSharp.Api.Extensions;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Quiz.CSharp.Api.Mapping;
using Quiz.CSharp.Api.Services;
using Quiz.CSharp.Api.Validators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpApi(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CategoryProfile).Assembly);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(SubmitAnswerRequestValidator).Assembly);

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IAnswerService, AnswerService>();
        services.AddScoped<IAnswerValidator, AnswerValidator>();

        return services;
    }
} 