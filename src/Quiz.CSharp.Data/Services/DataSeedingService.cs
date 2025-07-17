namespace Quiz.CSharp.Data.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quiz.CSharp.Data.Entities;

public class DataSeedingService(
    IServiceProvider serviceProvider,
    ILogger<DataSeedingService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting data seeding service...");

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CSharpDbContext>();

            // Check if data already exists
            if (await dbContext.Collections.AnyAsync(cancellationToken))
            {
                logger.LogInformation("Data already exists, skipping seeding.");
                return;
            }

            await SeedCollectionsAsync(dbContext, cancellationToken);
            await SeedQuestionsAsync(dbContext, cancellationToken);

            logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding data.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Data seeding service stopped.");
        return Task.CompletedTask;
    }

    private async Task SeedCollectionsAsync(CSharpDbContext dbContext, CancellationToken cancellationToken)
    {
        var collections = new[]
        {
            new Collection
            {
                Code = "csharp-basics",
                Title = "C# Fundamentals",
                Description = "Master the basics of C# programming language including syntax, data types, and control structures.",
                Icon = "🔤",
                SortOrder = 1,
                IsActive = true
            },
            new Collection
            {
                Code = "oop-concepts",
                Title = "Object-Oriented Programming",
                Description = "Learn OOP principles in C#: classes, inheritance, polymorphism, and encapsulation.",
                Icon = "️",
                SortOrder = 2,
                IsActive = true
            },
            new Collection
            {
                Code = "linq-queries",
                Title = "LINQ & Collections",
                Description = "Explore LINQ queries, collections, and data manipulation in C#.",
                Icon = "🔍",
                SortOrder = 3,
                IsActive = true
            }
        };

        dbContext.Collections.AddRange(collections);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Collections seeded successfully.");
    }

    private async Task SeedQuestionsAsync(CSharpDbContext dbContext, CancellationToken cancellationToken)
    {
        var collections = await dbContext.Collections.ToListAsync(cancellationToken);
        var questions = new List<Question>();

        // C# Basics Collection Questions
        var csharpBasics = collections.First(c => c.Code == "csharp-basics");
        
        // MCQ Questions
        questions.Add(new MCQQuestion
        {
            CollectionId = csharpBasics.Id,
            Subcategory = "Data Types",
            Difficulty = "Beginner",
            Prompt = "Which of the following is NOT a value type in C#?",
            EstimatedTimeMinutes = 2,
            IsActive = true,
            Options = new List<MCQOption>
            {
                new() { Id = "A", QuestionId = 0, Option = "int", IsCorrect = false, IsActive = true },
                new() { Id = "B", QuestionId = 0, Option = "string", IsCorrect = true, IsActive = true },
                new() { Id = "C", QuestionId = 0, Option = "bool", IsCorrect = false, IsActive = true },
                new() { Id = "D", QuestionId = 0, Option = "double", IsCorrect = false, IsActive = true }
            }
        });

        // True/False Questions
        questions.Add(new TrueFalseQuestion
        {
            CollectionId = csharpBasics.Id,
            Subcategory = "Variables",
            Difficulty = "Beginner",
            Prompt = "In C#, the 'var' keyword can only be used for local variables.",
            EstimatedTimeMinutes = 1,
            CorrectAnswer = true,
            IsActive = true
        });

        // Add questions to context
        dbContext.Questions.AddRange(questions);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Questions seeded successfully.");
    }
} 