namespace Quiz.CSharp.Data.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Models;
using System.Text.Json;

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

            await SeedFromJsonFilesAsync(dbContext, cancellationToken);

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

    private async Task SeedFromJsonFilesAsync(CSharpDbContext dbContext, CancellationToken cancellationToken)
    {
        var seedDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", "questions");
        
        if (!Directory.Exists(seedDirectory))
        {
            logger.LogWarning("Seed directory not found: {SeedDirectory}", seedDirectory);
            return;
        }

        var jsonFiles = Directory.GetFiles(seedDirectory, "*.json");
        logger.LogInformation("Found {Count} JSON files to process", jsonFiles.Length);

        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                await ProcessJsonFileAsync(dbContext, jsonFile, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing file: {FileName}", jsonFile);
            }
        }
    }

    private async Task ProcessJsonFileAsync(CSharpDbContext dbContext, string jsonFilePath, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(jsonFilePath);
        logger.LogInformation("Processing file: {FileName}", fileName);

        var jsonContent = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
        var seedData = JsonSerializer.Deserialize<SeedQuestionFile>(jsonContent);

        if (seedData?.Metadata == null || seedData.Questions == null)
        {
            logger.LogWarning("Invalid JSON structure in file: {FileName}", fileName);
            return;
        }

        // Create or get collection
        var collection = await GetOrCreateCollectionAsync(dbContext, seedData.Metadata, cancellationToken);

        // Process questions
        var questionId = 1;
        foreach (var seedQuestion in seedData.Questions)
        {
            try
            {
                await CreateQuestionFromSeedAsync(dbContext, collection, seedQuestion, questionId++, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating question {QuestionId} from file {FileName}", seedQuestion.Id, fileName);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully processed {QuestionCount} questions from {FileName}", 
            seedData.Questions.Count, fileName);
    }

    private async Task<Collection> GetOrCreateCollectionAsync(CSharpDbContext dbContext, SeedCollectionMetadata metadata, CancellationToken cancellationToken)
    {
        var existingCollection = await dbContext.Collections
            .FirstOrDefaultAsync(c => c.Code == metadata.Id, cancellationToken);

        if (existingCollection != null)
        {
            return existingCollection;
        }

        var collection = new Collection
        {
            Code = metadata.Id,
            Title = metadata.Title,
            Description = metadata.Description,
            Icon = metadata.Icon,
            SortOrder = metadata.CategoryId,
            IsActive = true
        };

        dbContext.Collections.Add(collection);
        await dbContext.SaveChangesAsync(cancellationToken);

        return collection;
    }

    private async Task CreateQuestionFromSeedAsync(CSharpDbContext dbContext, Collection collection, SeedQuestion seedQuestion, int questionId, CancellationToken cancellationToken)
    {
        Question question = seedQuestion.Type.ToLowerInvariant() switch
        {
            "mcq" => CreateMCQQuestion(collection, seedQuestion, questionId),
            "true_false" => CreateTrueFalseQuestion(collection, seedQuestion, questionId),
            "fill" => CreateFillQuestion(collection, seedQuestion, questionId),
            "error_spotting" => CreateErrorSpottingQuestion(collection, seedQuestion, questionId),
            "output_prediction" => CreateOutputPredictionQuestion(collection, seedQuestion, questionId),
            "code_writing" => CreateCodeWritingQuestion(collection, seedQuestion, questionId),
            _ => throw new NotSupportedException($"Question type '{seedQuestion.Type}' is not supported")
        };

        dbContext.Questions.Add(question);

        // Create MCQ options if it's an MCQ question
        if (seedQuestion.Type.ToLowerInvariant() == "mcq")
        {
            foreach (var option in seedQuestion.Options)
            {
                var isCorrect = seedQuestion.Answer.Contains(option.Id);
                var mcqOption = new MCQOption
                {
                    Id = option.Id,
                    QuestionId = questionId,
                    Option = option.Option,
                    IsCorrect = isCorrect,
                    IsActive = true
                };
                dbContext.MCQOptions.Add(mcqOption);
            }
        }

        // Create hints if any
        if (!string.IsNullOrEmpty(seedQuestion.Explanation))
        {
            var hint = new QuestionHint
            {
                Id = 0, // Will be auto-generated
                QuestionId = questionId,
                Hint = seedQuestion.Explanation,
                OrderIndex = 1,
                IsActive = true
            };
            dbContext.QuestionHints.Add(hint);
        }

        // Create test cases for code writing questions
        if (seedQuestion.Type.ToLowerInvariant() == "code_writing" && seedQuestion.TestCases.Any())
        {
            foreach (var (index, testCase) in seedQuestion.TestCases.Select((tc, i) => (i, tc)))
            {
                var dbTestCase = new TestCase
                {
                    Id = 0, // Will be auto-generated
                    QuestionId = questionId,
                    Input = testCase.Input,
                    ExpectedOutput = testCase.ExpectedOutput,
                    IsActive = true
                };
                dbContext.TestCases.Add(dbTestCase);
            }
        }
    }

    private MCQQuestion CreateMCQQuestion(Collection collection, SeedQuestion seedQuestion, int questionId)
    {
        return new MCQQuestion
        {
            Id = questionId,
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = "Beginner", // Default difficulty
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 2, // Default time
            IsActive = true
        };
    }

    private TrueFalseQuestion CreateTrueFalseQuestion(Collection collection, SeedQuestion seedQuestion, int questionId)
    {
        var correctAnswer = seedQuestion.Answer.FirstOrDefault()?.ToLowerInvariant() == "true";
        
        return new TrueFalseQuestion
        {
            Id = questionId,
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = "Beginner",
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 1,
            CorrectAnswer = correctAnswer,
            IsActive = true
        };
    }

    private FillQuestion CreateFillQuestion(Collection collection, SeedQuestion seedQuestion, int questionId)
    {
        return new FillQuestion
        {
            Id = questionId,
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = "Beginner",
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 3,
            CorrectAnswer = seedQuestion.Answer.FirstOrDefault() ?? string.Empty,
            FillHints = seedQuestion.Examples?.ToList() ?? new List<string>(),
            IsActive = true
        };
    }

    private ErrorSpottingQuestion CreateErrorSpottingQuestion(Collection collection, SeedQuestion seedQuestion, int questionId)
    {
        return new ErrorSpottingQuestion
        {
            Id = questionId,
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = "Intermediate",
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 5,
            CorrectAnswer = seedQuestion.Answer.FirstOrDefault() ?? string.Empty,
            IsActive = true
        };
    }

    private OutputPredictionQuestion CreateOutputPredictionQuestion(Collection collection, SeedQuestion seedQuestion, int questionId)
    {
        return new OutputPredictionQuestion
        {
            Id = questionId,
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = "Intermediate",
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 3,
            ExpectedOutput = seedQuestion.Answer.FirstOrDefault() ?? string.Empty,
            IsActive = true
        };
    }

    private CodeWritingQuestion CreateCodeWritingQuestion(Collection collection, SeedQuestion seedQuestion, int questionId)
    {
        return new CodeWritingQuestion
        {
            Id = questionId,
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = "Advanced",
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 10,
            Solution = seedQuestion.Solution,
            Examples = seedQuestion.Examples?.ToList() ?? new List<string>(),
            Rubric = seedQuestion.Rubric?.ToList() ?? new List<string>(),
            IsActive = true
        };
    }
} 