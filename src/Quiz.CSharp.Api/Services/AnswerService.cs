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

            // Update user progress
            await UpdateUserProgressAsync(currentUser.UserId, question.CollectionId, cancellationToken);
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

    private async Task UpdateUserProgressAsync(string userId, int collectionId, CancellationToken cancellationToken)
    {
        var (totalQuestions, answeredQuestions, correctAnswers) = 
            await repository.CalculateProgressStatsAsync(userId, collectionId, cancellationToken);

        var successRate = answeredQuestions > 0 ? (decimal)correctAnswers / answeredQuestions * 100 : 0;

        var existingProgress = await repository.GetUserProgressAsync(userId, collectionId, cancellationToken);
        
        if (existingProgress != null)
        {
            // Update existing progress
            existingProgress.TotalQuestions = totalQuestions;
            existingProgress.AnsweredQuestions = answeredQuestions;
            existingProgress.CorrectAnswers = correctAnswers;
            existingProgress.SuccessRate = Math.Round(successRate, 2);
            existingProgress.LastAnsweredAt = DateTime.UtcNow;
            existingProgress.UpdatedAt = DateTime.UtcNow;
            
            await repository.UpdateUserProgressAsync(existingProgress, cancellationToken);
        }
        else
        {
            // Create new progress record
            var newProgress = new UserProgress
            {
                UserId = userId,
                Username = currentUser.Username,
                Name = currentUser.Name,
                TelegramUsername = currentUser.TelegramUsername,
                CollectionId = collectionId,
                TotalQuestions = totalQuestions,
                AnsweredQuestions = answeredQuestions,
                CorrectAnswers = correctAnswers,
                SuccessRate = Math.Round(successRate, 2),
                LastAnsweredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            await repository.CreateUserProgressAsync(newProgress, cancellationToken);
        }

        logger.LogInformation("Updated progress for user {UserId} in collection {CollectionId}: {AnsweredQuestions}/{TotalQuestions} answered, {CorrectAnswers} correct",
            userId, collectionId, answeredQuestions, totalQuestions, correctAnswers);
    }
} 