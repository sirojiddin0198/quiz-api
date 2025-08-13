namespace Quiz.CSharp.Api.Services;

using Microsoft.Extensions.Logging;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.Shared.Authentication;
using Quiz.Shared.Common;
using Quiz.CSharp.Api.Services.Abstractions;

public sealed class AnswerService(
    IAnswerRepository answerRepository,
    IQuestionRepository questionRepository,
    IUserProgressRepository userProgressRepository,
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
        var question = await questionRepository.GetSingleOrDefaultAsync(questionId, cancellationToken);
        if (question is null)
            return Result<AnswerSubmissionResponse>.Failure("Question not found");

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

    public async Task<UserAnswerResponse?> GetLatestAnswerOrDefaultAsync(int questionId, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return null;

        var answer = await answerRepository.GetLatestAnswerOrDefaultAsync(currentUser.UserId, questionId, cancellationToken);
        return answer is not null ? new UserAnswerResponse
        {
            Answer = answer.Answer,
            SubmittedAt = answer.SubmittedAt,
            IsCorrect = answer.IsCorrect
        } : null;
    }

    private async Task UpdateUserProgressAsync(
        string userId,
        int collectionId,
        CancellationToken cancellationToken)
    {
        var (totalQuestions, answeredQuestions, correctAnswers) = 
            await userProgressRepository.CalculateProgressStatsAsync(userId, collectionId, cancellationToken);

        var successRate = answeredQuestions > 0 ? (decimal)correctAnswers / answeredQuestions * 100 : 0;

        var existingProgress = await userProgressRepository.GetUserProgressOrDefaultAsync(userId, collectionId, cancellationToken);
        
        if (existingProgress != null)
        {
            // Update existing progress
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
            
            await userProgressRepository.CreateUserProgressAsync(newProgress, cancellationToken);
        }

        logger.LogInformation("Updated progress for user {UserId} in collection {CollectionId}: {AnsweredQuestions}/{TotalQuestions} answered, {CorrectAnswers} correct",
            userId, collectionId, answeredQuestions, totalQuestions, correctAnswers);
    }
} 