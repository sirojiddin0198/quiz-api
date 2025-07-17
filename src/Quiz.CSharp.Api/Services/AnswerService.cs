namespace Quiz.CSharp.Api.Services;

using Microsoft.Extensions.Logging;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Services;
using Quiz.Shared.Authentication;
using Quiz.Shared.Common;

public sealed class AnswerService(
    ICSharpRepository repository,
    IAnswerValidator validator,
    ICurrentUser currentUser,
    ILogger<AnswerService> logger) : IAnswerService
{
    public async Task<Result<AnswerSubmissionResponse>> SubmitAnswerAsync(
        int questionId,
        string answer,
        int timeSpentSeconds,
        CancellationToken cancellationToken = default)
    {
        var question = await repository.GetQuestionByIdAsync(questionId, cancellationToken);
        if (question is null)
            return Result<AnswerSubmissionResponse>.Failure("Question not found");

        var isCorrect = await validator.ValidateAnswerAsync(question, answer, cancellationToken);

        if (currentUser.IsAuthenticated && currentUser.UserId is not null)
        {
            var attemptNumber = await repository.GetNextAttemptNumberAsync(
                currentUser.UserId,
                questionId,
                cancellationToken);

            var userAnswer = new UserAnswer
            {
                UserId = currentUser.UserId,
                QuestionId = questionId,
                Answer = answer,
                IsCorrect = isCorrect,
                TimeSpentSeconds = timeSpentSeconds,
                AttemptNumber = attemptNumber
            };

            await repository.SaveAnswerAsync(userAnswer, cancellationToken);
        }

        logger.LogInformation("Answer submitted for question {QuestionId}, correct: {IsCorrect}",
            questionId, isCorrect);

        return Result<AnswerSubmissionResponse>.Success(new AnswerSubmissionResponse
        {
            Success = true,
            IsCorrect = isCorrect
        });
    }

    public async Task<UserAnswerResponse?> GetLatestAnswerAsync(int questionId, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return null;

        var answer = await repository.GetLatestAnswerAsync(currentUser.UserId, questionId, cancellationToken);
        return answer is not null ? new UserAnswerResponse
        {
            Answer = answer.Answer,
            SubmittedAt = answer.SubmittedAt,
            IsCorrect = answer.IsCorrect
        } : null;
    }
} 