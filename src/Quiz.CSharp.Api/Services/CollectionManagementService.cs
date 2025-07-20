namespace Quiz.CSharp.Api.Services;

using Microsoft.Extensions.Logging;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Contracts.Requests;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Services;
using Quiz.CSharp.Data.ValueObjects;
using Quiz.Shared.Common;

public sealed class CollectionManagementService(
    ICSharpRepository repository,
    ILogger<CollectionManagementService> logger) : ICollectionManagementService
{
    public async Task<Result<CreateCollectionResponse>> CreateCollectionWithQuestionsAsync(
        CreateCollectionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate collection doesn't already exist
            if (await repository.CollectionExistsAsync(request.Code, cancellationToken))
            {
                return Result<CreateCollectionResponse>.Failure($"Collection with code '{request.Code}' already exists");
            }

            // Create collection entity
            var collection = new Collection
            {
                Code = request.Code,
                Title = request.Title,
                Description = request.Description,
                Icon = request.Icon,
                SortOrder = request.SortOrder,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Save collection first to get the ID
            var createdCollection = await repository.CreateCollectionAsync(collection, cancellationToken);

            // Create questions
            var questionsCreated = 0;
            foreach (var questionRequest in request.Questions)
            {
                var question = CreateQuestionFromRequest(questionRequest, createdCollection.Id);
                if (question != null)
                {
                    await repository.CreateQuestionAsync(question, cancellationToken);
                    questionsCreated++;
                }
                else
                {
                    logger.LogWarning("Failed to create question of type {Type}", questionRequest.Type);
                }
            }

            logger.LogInformation("Created collection {Code} with {QuestionCount} questions", 
                request.Code, questionsCreated);

            var response = new CreateCollectionResponse
            {
                Id = createdCollection.Id,
                Code = createdCollection.Code,
                Title = createdCollection.Title,
                Description = createdCollection.Description,
                Icon = createdCollection.Icon,
                SortOrder = createdCollection.SortOrder,
                QuestionsCreated = questionsCreated,
                CreatedAt = createdCollection.CreatedAt
            };

            return Result<CreateCollectionResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating collection {Code}", request.Code);
            return Result<CreateCollectionResponse>.Failure("An error occurred while creating the collection");
        }
    }

    private static Question? CreateQuestionFromRequest(CreateQuestionRequest request, int collectionId)
    {
        var questionType = GetQuestionTypeFromString(request.Type);
        if (questionType == null)
            return null;

        return questionType.Value switch
        {
            QuestionType.MCQ => new MCQQuestion
            {
                CollectionId = collectionId,
                Subcategory = request.Subcategory,
                Difficulty = request.Difficulty,
                Prompt = request.Prompt,
                EstimatedTimeMinutes = request.EstimatedTimeMinutes,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            QuestionType.TrueFalse => new TrueFalseQuestion
            {
                CollectionId = collectionId,
                Subcategory = request.Subcategory,
                Difficulty = request.Difficulty,
                Prompt = request.Prompt,
                EstimatedTimeMinutes = request.EstimatedTimeMinutes,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            QuestionType.Fill => new FillQuestion
            {
                CollectionId = collectionId,
                Subcategory = request.Subcategory,
                Difficulty = request.Difficulty,
                Prompt = request.Prompt,
                EstimatedTimeMinutes = request.EstimatedTimeMinutes,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            QuestionType.ErrorSpotting => new ErrorSpottingQuestion
            {
                CollectionId = collectionId,
                Subcategory = request.Subcategory,
                Difficulty = request.Difficulty,
                Prompt = request.Prompt,
                EstimatedTimeMinutes = request.EstimatedTimeMinutes,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            QuestionType.OutputPrediction => new OutputPredictionQuestion
            {
                CollectionId = collectionId,
                Subcategory = request.Subcategory,
                Difficulty = request.Difficulty,
                Prompt = request.Prompt,
                EstimatedTimeMinutes = request.EstimatedTimeMinutes,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            QuestionType.CodeWriting => new CodeWritingQuestion
            {
                CollectionId = collectionId,
                Subcategory = request.Subcategory,
                Difficulty = request.Difficulty,
                Prompt = request.Prompt,
                EstimatedTimeMinutes = request.EstimatedTimeMinutes,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            _ => null
        };
    }

    private static QuestionType? GetQuestionTypeFromString(string typeString)
    {
        return typeString.ToLowerInvariant() switch
        {
            "mcq" => QuestionType.MCQ,
            "true_false" => QuestionType.TrueFalse,
            "fill" => QuestionType.Fill,
            "error_spotting" => QuestionType.ErrorSpotting,
            "output_prediction" => QuestionType.OutputPrediction,
            "code_writing" => QuestionType.CodeWriting,
            _ => null
        };
    }
} 