using Quiz.CSharp.Data.Data;
using Quiz.CSharp.Data.Repositories.Abstractions;

namespace Quiz.CSharp.Data.Repositories;

public sealed class AnswerRepository(ICSharpDbContext context) : IAnswerRepository
{
    public async Task SaveAnswerAsync(UserAnswer answer, CancellationToken cancellationToken = default)
    {
        context.UserAnswers.Add(answer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserAnswer?> GetLatestAnswerOrDefaultAsync(
        string userId,
        int questionId,
        CancellationToken cancellationToken = default)
        => await context.UserAnswers
            .Where(ua => ua.UserId == userId && ua.QuestionId == questionId)
            .OrderByDescending(ua => ua.SubmittedAt)
            .FirstOrDefaultAsync(cancellationToken);
            
    public async Task<int> GetNextAttemptNumberAsync(
        string userId,
        int questionId,
        CancellationToken cancellationToken = default)
    {
        var lastAttempt = await context.UserAnswers
            .Where(ua => ua.UserId == userId && ua.QuestionId == questionId)
            .OrderByDescending(ua => ua.AttemptNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return (lastAttempt?.AttemptNumber ?? 0) + 1;
    }
}