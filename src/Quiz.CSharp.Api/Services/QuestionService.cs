namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Services;
using Quiz.Shared.Authentication;
using Quiz.Shared.Common;

public sealed class QuestionService(
    ICSharpRepository repository,
    IMapper mapper,
    ICurrentUser currentUser) : IQuestionService
{
    public async Task<PaginatedResult<QuestionResponse>> GetQuestionsByCategoryAsync(
        string categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var questions = await repository.GetQuestionsByCategoryAsync(categoryId, page, pageSize, cancellationToken);
        var responses = new List<QuestionResponse>();

        foreach (var question in questions.Items)
        {
            var response = mapper.Map<QuestionResponse>(question);

            if (currentUser.IsAuthenticated && currentUser.UserId is not null)
            {
                var previousAnswer = await repository.GetLatestAnswerAsync(
                    currentUser.UserId,
                    question.Id,
                    cancellationToken);

                if (previousAnswer is not null)
                {
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
            }

            responses.Add(response);
        }

        return new PaginatedResult<QuestionResponse>(responses, questions.TotalCount, questions.Page, questions.PageSize);
    }

    public async Task<List<QuestionResponse>> GetPreviewQuestionsAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        var questions = await repository.GetPreviewQuestionsAsync(categoryId, cancellationToken);
        return mapper.Map<List<QuestionResponse>>(questions);
    }

    public async Task<QuestionResponse?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default)
    {
        var question = await repository.GetQuestionByIdAsync(questionId, cancellationToken);
        return question is not null ? mapper.Map<QuestionResponse>(question) : null;
    }
} 