namespace Quiz.CSharp.Api.Services;

using Microsoft.Extensions.Logging;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.Shared.Authentication;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.Infrastructure.Exceptions;

public sealed class AnswerService(
    IAnswerRepository answerRepository,
    IQuestionRepository questionRepository,
    IUserProgressRepository userProgressRepository,
    IAnswerValidator validator,
    ICurrentUser currentUser,
    IMapper mapper,
    ILogger<AnswerService> logger) : IAnswerService
{
    public async Task<AnswerSubmissionResponse> SubmitAnswerAsync(
        int questionId,
        string answer,
        int timeSpentSeconds,
        CancellationToken cancellationToken = default)
    {
        var question = await questionRepository.GetSingleOrDefaultAsync(questionId, cancellationToken);
        if (question is null)
            throw new CustomNotFoundException("Question not found");

        var isCorrect = await validator.ValidateAnswerAsync(question, answer, cancellationToken);

        if (currentUser.IsAuthenticated && currentUser.UserId is not null)
        {
            var attemptNumber = await answerRepository.GetNextAttemptNumberAsync(
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

            await answerRepository.SaveAnswerAsync(userAnswer, cancellationToken);

            await UpdateUserProgressAsync(
                currentUser.UserId,
                question.CollectionId,
                cancellationToken);
        }

        logger.LogInformation("Answer submitted for question {QuestionId}, correct: {IsCorrect}",
            questionId, isCorrect);

        return new AnswerSubmissionResponse
        {
            Success = true,
            IsCorrect = isCorrect
        };
    }

    public async Task<UserAnswerResponse> GetLatestAnswerAsync(int questionId, CancellationToken cancellationToken = default)
    {
        if (currentUser is not { IsAuthenticated: true, UserId: not null })
            throw new CustomUnauthorizedException("User not authenticated");

        var answer = await answerRepository.GetLatestAnswerOrDefaultAsync(
            currentUser.UserId,
            questionId,
            cancellationToken)
            ?? throw new CustomNotFoundException("Answer not found");

        return new UserAnswerResponse
        {
            Answer = answer.Answer,
            SubmittedAt = answer.SubmittedAt,
            IsCorrect = answer.IsCorrect
        };
    }

    private async Task UpdateUserProgressAsync(
        string userId,
        int collectionId,
        CancellationToken cancellationToken)
    {
        var (totalQuestions, answeredQuestions, correctAnswers) =
            await userProgressRepository.CalculateProgressStatsAsync(
                userId,
                collectionId,
                cancellationToken);

        var successRate = answeredQuestions > 0 ? (decimal)correctAnswers / answeredQuestions * 100 : 0;

        var existingProgress = await userProgressRepository.GetUserProgressOrDefaultAsync(
            userId,
            collectionId,
            cancellationToken);

        if (existingProgress != null)
        {
            existingProgress.TotalQuestions = totalQuestions;
            existingProgress.AnsweredQuestions = answeredQuestions;
            existingProgress.CorrectAnswers = correctAnswers;
            existingProgress.SuccessRate = Math.Round(successRate, 2);
            existingProgress.LastAnsweredAt = DateTime.UtcNow;
            existingProgress.UpdatedAt = DateTime.UtcNow;

            await userProgressRepository.UpdateUserProgressAsync(existingProgress, cancellationToken);
        }
        else
        {
            var newProgress = mapper.Map<UserProgress>(
                (currentUser,
                userId,
                collectionId,
                totalQuestions,
                answeredQuestions,
                correctAnswers,
                successRate));

            await userProgressRepository.CreateUserProgressAsync(newProgress, cancellationToken);
        }

        logger.LogInformation(
            @"User {UserId}, Collection {CollectionId}, {Answered}/{Total} answered, {Correct} correct",
            userId,
            collectionId,
            answeredQuestions,
            totalQuestions,
            correctAnswers);
    }
}