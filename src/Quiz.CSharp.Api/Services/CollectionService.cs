namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Contracts.Requests;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.Shared.Authentication;
using Quiz.CSharp.Data.ValueObjects;
using Quiz.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

public sealed class CollectionService(
    ICollectionRepository collectionRepository,
    IUserProgressRepository userProgressRepository,
    IQuestionRepository questionRepository,
    IMapper mapper,
    ICurrentUser currentUser,
    ILogger<CollectionService> logger) : ICollectionService
{
    public async Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var collectionsWithCounts = await collectionRepository.GetCollectionsWithQuestionCountAsync(cancellationToken);
        var responses = new List<CollectionResponse>();

        foreach (var collectionWithCount in collectionsWithCounts)
        {
            var response = mapper.Map<CollectionResponse>(collectionWithCount.Collection)
                with { TotalQuestions = collectionWithCount.QuestionCount };

            if (currentUser.IsAuthenticated && currentUser.UserId is not null)
            {
                var userProgress = await userProgressRepository.GetUserProgressOrDefaultAsync(
                    currentUser.UserId,
                    collectionWithCount.Collection.Id,
                    cancellationToken);

                if (userProgress is not null)
                    response = response with
                    {
                        UserProgress = mapper.Map<UserProgressResponse>(userProgress)
                    };
            }

            responses.Add(response);
        }

        return responses;
    }

    public async Task<CreateCollectionResponse> CreateCollectionWithQuestionsAsync(
        CreateCollectionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await collectionRepository.CollectionExistsAsync(request.Code, cancellationToken))
            throw new CustomConflictException($"Collection with code '{request.Code}' already exists");

        try
        {
            var collection = mapper.Map<Collection>(request);

            var createdCollection = await collectionRepository.CreateCollectionAsync(collection, cancellationToken);

            var questionsCreated = 0;
            foreach (var questionRequest in request.Questions)
            {
                var question = CreateQuestionFromRequest(questionRequest, createdCollection.Id, mapper);
                if (question is not null)
                {
                    await questionRepository.CreateQuestionAsync(question, cancellationToken);
                    questionsCreated++;
                }
                else
                    logger.LogWarning("Failed to create question of type {Type}", questionRequest.Type);
            }

            logger.LogInformation("Created collection {Code} with {QuestionCount} questions",
                request.Code, questionsCreated);

            var response = mapper.Map<CreateCollectionResponse>(createdCollection);
            response.QuestionsCreated = questionsCreated;

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating collection {Code}", request.Code);
            throw new CustomBadRequestException("An error occurred while creating the collection");
        }
    }

    private static Question? CreateQuestionFromRequest(CreateQuestionRequest request, int collectionId, IMapper mapper)
    {
        var questionType = GetQuestionTypeFromString(request.Type);
        if (questionType == null) return null;

        Question? question = questionType.Value switch
        {
            QuestionType.MCQ => mapper.Map<MCQQuestion>(request),
            QuestionType.TrueFalse=> mapper.Map<TrueFalseQuestion>(request),
            QuestionType.Fill=> mapper.Map<FillQuestion>(request),
            QuestionType.ErrorSpotting=> mapper.Map<ErrorSpottingQuestion>(request),
            QuestionType.OutputPrediction=> mapper.Map<OutputPredictionQuestion>(request),
             QuestionType.CodeWriting=> mapper.Map<CodeWritingQuestion>(request),
            _ => null
        };

        if (question is not null)
            question.CollectionId = collectionId;
        return question;
    }

    private static QuestionType? GetQuestionTypeFromString(string typeString) =>
        typeString.ToLowerInvariant() switch
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