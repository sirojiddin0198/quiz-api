namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.Shared.Authentication;
using Quiz.Shared.Common;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.CSharp.Api.Contracts.Requests;
using Quiz.CSharp.Data.Models;
using Quiz.CSharp.Data.ValueObjects;
using Quiz.CSharp.Data.Entities;
using Quiz.Infrastructure.Exceptions;

public sealed class QuestionService(
    IAnswerRepository answerRepository,
    IQuestionRepository questionRepository,
    ICollectionRepository collectionRepository,
    IMapper mapper,
    ICurrentUser currentUser) : IQuestionService
{
    public async Task<PaginatedResult<QuestionResponse>> GetQuestionsByCollectionAsync(
        int collectionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var questions = await questionRepository.GetQuestionsByCollectionAsync(
            collectionId,
            page,
            pageSize,
            cancellationToken);
        var responses = new List<QuestionResponse>();

        foreach (var question in questions.Items)
        {
            var response = mapper.Map<QuestionResponse>(question);

            if (currentUser.IsAuthenticated && currentUser.UserId is not null)
            {
                var previousAnswer = await answerRepository.GetLatestAnswerOrDefaultAsync(
                    currentUser.UserId,
                    question.Id,
                    cancellationToken);

                if (previousAnswer is not null)
                    response = response with
                    {
                        PreviousAnswer = new PreviousAnswerResponse
                        {
                            Answer = previousAnswer.Answer,
                            SubmittedAt = previousAnswer.SubmittedAt,
                            IsCorrect = previousAnswer.IsCorrect
                        }
                    };
            }

            responses.Add(response);
        }

        return new PaginatedResult<QuestionResponse>(
            responses,
            questions.TotalCount,
            questions.Page,
            questions.PageSize);
    }

    public async Task<List<QuestionResponse>> GetPreviewQuestionsAsync(
        int collectionId,
        CancellationToken cancellationToken = default)
    {
        var questions = await questionRepository.GetPreviewQuestionsAsync(collectionId, cancellationToken);
        return mapper.Map<List<QuestionResponse>>(questions);
    }

    public async Task<Result<CreateQuestionResponse>> CreateQuestionAsync(CreateQuestionModel model, CancellationToken cancellationToken = default)
    {
        if (await collectionRepository.ExistAsync(model.CollectionId, cancellationToken) is false)
            throw new CustomNotFoundException($"Collection with {model.CollectionId} id doesn't exist");

        var question = CreateQuestionFromModel(model) ?? throw new CustomBadRequestException($"Invalid question type: '{model.Type}'");
        
        var createdQuestion = await questionRepository.CreateQuestionAsync(question, cancellationToken);
        var response = mapper.Map<CreateQuestionResponse>(createdQuestion);
        return Result<CreateQuestionResponse>.Success(response);
    }

    private Question? CreateQuestionFromModel(CreateQuestionModel model)
    {
        var questionType = GetQuestionTypeFromString(model.Type);
        if (questionType is null)
            return null;

        return questionType.Value switch
        {
            QuestionType.MCQ => mapper.Map<MCQQuestion>(model),
            QuestionType.TrueFalse=> mapper.Map<TrueFalseQuestion>(model),
            QuestionType.Fill=> mapper.Map<FillQuestion>(model),
            QuestionType.ErrorSpotting=> mapper.Map<ErrorSpottingQuestion>(model),
            QuestionType.OutputPrediction=> mapper.Map<OutputPredictionQuestion>(model),
             QuestionType.CodeWriting=> mapper.Map<CodeWritingQuestion>(model),
            _ => null};
    }

    private static QuestionType? GetQuestionTypeFromString(string type)
    {
        return type.ToLowerInvariant() switch
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