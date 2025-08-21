namespace Quiz.CSharp.Data.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quiz.CSharp.Data.Data;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Models;
using System.Text.Json;

public class DataSeedingService(
    IServiceProvider serviceProvider,
    ILogger<DataSeedingService> logger) : IHostedService
{
    internal class QuestionMetadataBase
    {
        public string? CodeBefore { get; set; }
        public string? CodeAfter { get; set; }
        public List<QuestionHintData> Hints { get; set; } = [];
        public string? Explanation { get; set; }
    }

    internal class MCQMetadata : QuestionMetadataBase
    {
        public List<MCQOptionData> Options { get; set; } = [];
        public List<string> CorrectAnswerIds { get; set; } = [];
    }

    internal class TrueFalseMetadata : QuestionMetadataBase
    {
        public bool CorrectAnswer { get; set; }
    }

    internal class FillMetadata : QuestionMetadataBase
    {
        public string? CodeWithBlank { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<string> FillHints { get; set; } = [];
    }

    internal class ErrorSpottingMetadata : QuestionMetadataBase
    {
        public string? CodeWithError { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    internal class OutputPredictionMetadata : QuestionMetadataBase
    {
        public string? Snippet { get; set; }
        public string ExpectedOutput { get; set; } = string.Empty;
    }

    internal class CodeWritingMetadata : QuestionMetadataBase
    {
        public string? Solution { get; set; }
        public List<string> Examples { get; set; } = [];
        public List<string> Rubric { get; set; } = [];
        public List<TestCaseData> TestCases { get; set; } = [];
    }

    internal class MCQOptionData
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    internal class QuestionHintData
    {
        public string Hint { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }

    internal class TestCaseData
    {
        public string Input { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
    }

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
        
        if (Directory.Exists(seedDirectory) is false)
        {
            logger.LogWarning("Seed directory not found: {SeedDirectory}", seedDirectory);
            return;
        }

        var jsonFiles = Directory.GetFiles(seedDirectory, "*.json");
        logger.LogInformation("Found {Count} JSON files to process", jsonFiles.Length);

        var filesWithMetadata = new List<(string FilePath, int CategoryId)>();
        
        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                var jsonContent = await File.ReadAllTextAsync(jsonFile, cancellationToken);
                var seedData = JsonSerializer.Deserialize<SeedQuestionFile>(jsonContent);
                
                if (seedData?.Metadata != null)
                    filesWithMetadata.Add((jsonFile, seedData.Metadata.CategoryId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading metadata from file: {FileName}", jsonFile);
            }
        }

        filesWithMetadata.Sort((a, b) => a.CategoryId.CompareTo(b.CategoryId));
        
        logger.LogInformation("Processing files in difficulty order: {Order}", 
            string.Join(" → ", filesWithMetadata.Select(f => f.CategoryId)));

        foreach (var (jsonFile, categoryId) in filesWithMetadata)
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

    private async Task ProcessJsonFileAsync(
        CSharpDbContext dbContext,
        string jsonFilePath,
        CancellationToken cancellationToken)
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

        var collection = await GetOrCreateCollectionAsync(
            dbContext,
            seedData.Metadata,
            cancellationToken);

        var sortedQuestions = SortQuestionsByType(seedData.Questions);
        
        logger.LogInformation("Processing {QuestionCount} questions in type order for collection {CollectionCode}", 
            sortedQuestions.Count, collection.Code);

        var processedCount = 0;
        var skippedCount = 0;
        foreach (var seedQuestion in sortedQuestions)
        {
            try
            {
                var wasCreated = await CreateQuestionFromSeedAsync(
                    dbContext,
                    collection,
                    seedQuestion,
                    cancellationToken);
                if (wasCreated)
                {
                    await dbContext.SaveChangesAsync(cancellationToken);
                    processedCount++;
                }
                else
                    skippedCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error creating question {QuestionId} from file {FileName}",
                    seedQuestion.Id,
                    fileName);
            }
        }

        logger.LogInformation(@"Successfully processed {ProcessedCount} questions, 
            skipped {SkippedCount} existing questions from {FileName}", 
            processedCount, skippedCount, fileName);
    }

    private static List<SeedQuestion> SortQuestionsByType(List<SeedQuestion> questions)
    {
        var typeOrder = new Dictionary<string, int>
        {
            { "true_false", 1 },
            { "mcq", 2 },
            { "error_spotting", 3 },
            { "fill", 4 },
            { "output_prediction", 5 },
            { "code_writing", 6 }
        };

        return [.. questions.OrderBy(q => typeOrder.GetValueOrDefault(q.Type.ToLowerInvariant(), 999))];
    }

    private async Task<Collection> GetOrCreateCollectionAsync(
        CSharpDbContext dbContext,
        SeedCollectionMetadata metadata,
        CancellationToken cancellationToken)
    {
        var existingCollection = await dbContext.Collections
            .FirstOrDefaultAsync(c => c.Code == metadata.Id, cancellationToken);

        if (existingCollection is not null) return existingCollection;

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

    private async Task<bool> CreateQuestionFromSeedAsync(
        CSharpDbContext dbContext,
        Collection collection,
        SeedQuestion seedQuestion,
        CancellationToken cancellationToken)
    {
        var existingQuestion = await dbContext.Questions
            .FirstOrDefaultAsync(q => q.CollectionId == collection.Id && q.Prompt == seedQuestion.Prompt, cancellationToken);

        if (existingQuestion is not null)
        {
            logger.LogDebug("Question already exists in collection {CollectionId} with prompt: {Prompt}", 
                collection.Id,
                seedQuestion.Prompt);
            return false;
        }

        Question question = seedQuestion.Type.ToLowerInvariant() switch
        {
            "mcq" => CreateMCQQuestion(collection, seedQuestion),
            "true_false" => CreateTrueFalseQuestion(collection, seedQuestion),
            "fill" => CreateFillQuestion(collection, seedQuestion),
            "error_spotting" => CreateErrorSpottingQuestion(collection, seedQuestion),
            "output_prediction" => CreateOutputPredictionQuestion(collection, seedQuestion),
            "code_writing" => CreateCodeWritingQuestion(collection, seedQuestion),
            _ => throw new NotSupportedException($"Question type '{seedQuestion.Type}' is not supported")
        };

        dbContext.Questions.Add(question);
        return true;
    }

    private MCQQuestion CreateMCQQuestion(Collection collection, SeedQuestion seedQuestion)
    {
        var metadata = new MCQMetadata
        {
            CodeBefore = seedQuestion.CodeBefore,
            CodeAfter = seedQuestion.CodeAfter,
            Options = seedQuestion.Options.Select(o => new MCQOptionData
            {
                Id = o.Id,
                Text = o.Option,
                IsCorrect = seedQuestion.Answer.Contains(o.Id)
            }).ToList(),
            CorrectAnswerIds = seedQuestion.Answer,
            Explanation = seedQuestion.Explanation,
            Hints = CreateHintsFromExplanation(seedQuestion.Explanation)
        };

        return new MCQQuestion
        {
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = DetermineDifficulty(seedQuestion),
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 2,
            Metadata = JsonSerializer.Serialize(metadata),
            IsActive = true
        };
    }

    private TrueFalseQuestion CreateTrueFalseQuestion(Collection collection, SeedQuestion seedQuestion)
    {
        var correctAnswer = seedQuestion.Answer.FirstOrDefault()?.ToLowerInvariant() == "true";
        
        var metadata = new TrueFalseMetadata
        {
            CodeBefore = seedQuestion.CodeBefore,
            CodeAfter = seedQuestion.CodeAfter,
            CorrectAnswer = correctAnswer,
            Explanation = seedQuestion.Explanation,
            Hints = CreateHintsFromExplanation(seedQuestion.Explanation)
        };

        return new TrueFalseQuestion
        {
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = DetermineDifficulty(seedQuestion),
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 1,
            Metadata = JsonSerializer.Serialize(metadata),
            IsActive = true
        };
    }

    private FillQuestion CreateFillQuestion(Collection collection, SeedQuestion seedQuestion)
    {
        var metadata = new FillMetadata
        {
            CodeBefore = seedQuestion.CodeBefore,
            CodeAfter = seedQuestion.CodeAfter,
            CodeWithBlank = seedQuestion.CodeWithBlank,
            CorrectAnswer = seedQuestion.Answer.FirstOrDefault() ?? string.Empty,
            FillHints = seedQuestion.Examples?.ToList() ?? new List<string>(),
            Explanation = seedQuestion.Explanation,
            Hints = CreateHintsFromExplanation(seedQuestion.Explanation)
        };

        return new FillQuestion
        {
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = DetermineDifficulty(seedQuestion),
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 3,
            Metadata = JsonSerializer.Serialize(metadata),
            IsActive = true
        };
    }

    private ErrorSpottingQuestion CreateErrorSpottingQuestion(Collection collection, SeedQuestion seedQuestion)
    {
        var metadata = new ErrorSpottingMetadata
        {
            CodeBefore = seedQuestion.CodeBefore,
            CodeAfter = seedQuestion.CodeAfter,
            CodeWithError = seedQuestion.CodeWithError,
            CorrectAnswer = seedQuestion.Answer.FirstOrDefault() ?? string.Empty,
            Explanation = seedQuestion.Explanation,
            Hints = CreateHintsFromExplanation(seedQuestion.Explanation)
        };

        return new ErrorSpottingQuestion
        {
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = DetermineDifficulty(seedQuestion, "Intermediate"),
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 5,
            Metadata = JsonSerializer.Serialize(metadata),
            IsActive = true
        };
    }

    private OutputPredictionQuestion CreateOutputPredictionQuestion(Collection collection, SeedQuestion seedQuestion)
    {
        var metadata = new OutputPredictionMetadata
        {
            CodeBefore = seedQuestion.CodeBefore,
            CodeAfter = seedQuestion.CodeAfter,
            Snippet = seedQuestion.Snippet,
            ExpectedOutput = seedQuestion.Answer.FirstOrDefault() ?? string.Empty,
            Explanation = seedQuestion.Explanation,
            Hints = CreateHintsFromExplanation(seedQuestion.Explanation)
        };

        return new OutputPredictionQuestion
        {
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = DetermineDifficulty(seedQuestion, "Intermediate"),
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 3,
            Metadata = JsonSerializer.Serialize(metadata),
            IsActive = true
        };
    }

    private CodeWritingQuestion CreateCodeWritingQuestion(Collection collection, SeedQuestion seedQuestion)
    {
        var metadata = new CodeWritingMetadata
        {
            CodeBefore = seedQuestion.CodeBefore,
            CodeAfter = seedQuestion.CodeAfter,
            Solution = seedQuestion.Solution,
            Examples = seedQuestion.Examples?.ToList() ?? new List<string>(),
            Rubric = seedQuestion.Rubric?.ToList() ?? new List<string>(),
            TestCases = seedQuestion.TestCases.Select(tc => new TestCaseData
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput
            }).ToList(),
            Explanation = seedQuestion.Explanation,
            Hints = CreateHintsFromExplanation(seedQuestion.Explanation)
        };

        return new CodeWritingQuestion
        {
            CollectionId = collection.Id,
            Subcategory = seedQuestion.Metadata.Subcategory,
            Difficulty = DetermineDifficulty(seedQuestion, "Advanced"),
            Prompt = seedQuestion.Prompt,
            EstimatedTimeMinutes = 10,
            Metadata = JsonSerializer.Serialize(metadata),
            IsActive = true
        };
    }

    private List<QuestionHintData> CreateHintsFromExplanation(string? explanation)
    {
        var hints = new List<QuestionHintData>();
        
        if (string.IsNullOrWhiteSpace(explanation) is false)
        {
            hints.Add(new QuestionHintData
            {
                Hint = explanation,
                OrderIndex = 1
            });
        }

        return hints;
    }

    private string DetermineDifficulty(SeedQuestion seedQuestion, string defaultDifficulty = "Beginner")
        => defaultDifficulty;
} 