namespace Quiz.CSharp.Data.Repositories.Abstractions;

public interface IUserProgressRepository
{
    Task<UserProgress?> GetUserProgressOrDefaultAsync(
        string userId,
        int collectionId,
        CancellationToken cancellationToken = default);
    Task UpdateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task CreateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task<(int totalQuestions, int answeredQuestions, int correctAnswers)> CalculateProgressStatsAsync(
        string userId,
        int collectionId,
        CancellationToken cancellationToken = default);
    Task<PaginatedResult<UserProgress>> GetAllUserProgressesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<IGrouping<string, UserProgress>>> GetUserProgressesGroupedByUserAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}