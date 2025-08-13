namespace Quiz.CSharp.Data.Repositories.Abstractions;

public interface IAnswerRepository
{
    Task SaveAnswerAsync(UserAnswer answer, CancellationToken cancellationToken = default);
    Task<UserAnswer?> GetLatestAnswerOrDefaultAsync(
        string userId,
        int questionId,
        CancellationToken cancellationToken = default);
    Task<int> GetNextAttemptNumberAsync(
        string userId,
        int questionId,
        CancellationToken cancellationToken = default);
}