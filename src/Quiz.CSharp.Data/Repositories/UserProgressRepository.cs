using Quiz.CSharp.Data.Data;
using Quiz.CSharp.Data.Repositories.Abstractions;

namespace Quiz.CSharp.Data.Repositories;

public sealed class UserProgressRepository(ICSharpDbContext context) : IUserProgressRepository
{
    public async Task<UserProgress?> GetUserProgressOrDefaultAsync(
        string userId,
        int collectionId,
        CancellationToken cancellationToken = default)
        => await context.UserProgress
            .Include(up => up.Collection)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.CollectionId == collectionId, cancellationToken);

    public async Task UpdateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default)
    {
        context.UserProgress.Update(progress);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default)
    {
        context.UserProgress.Add(progress);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(int totalQuestions, int answeredQuestions, int correctAnswers)> CalculateProgressStatsAsync(
        string userId,
        int collectionId,
        CancellationToken cancellationToken = default)
    {
        var totalQuestions = await context.Questions
            .Where(q => q.CollectionId == collectionId && q.IsActive)
            .CountAsync(cancellationToken);

        var answeredStats = await context.UserAnswers
            .Where(ua => ua.UserId == userId)
            .Join(context.Questions,
                ua => ua.QuestionId,
                q => q.Id,
                (ua, q) => new { ua, q })
            .Where(joined => joined.q.CollectionId == collectionId && joined.q.IsActive)
            .GroupBy(joined => joined.ua.QuestionId)
            .Select(g => new
            {
                QuestionId = g.Key,
                IsCorrect = g.OrderByDescending(x => x.ua.SubmittedAt).First().ua.IsCorrect
            })
            .ToListAsync(cancellationToken);

        var answeredQuestions = answeredStats.Count;
        var correctAnswers = answeredStats.Count(s => s.IsCorrect);

        return (totalQuestions, answeredQuestions, correctAnswers);
    }
    
    public async Task<PaginatedResult<UserProgress>> GetAllUserProgressesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.UserProgress
            .Include(up => up.Collection)
            .Where(up => up.IsActive)
            .OrderByDescending(up => up.UpdatedAt ?? up.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<UserProgress>(items, totalCount, page, pageSize);
    }

    public async Task<PaginatedResult<IGrouping<string, UserProgress>>> GetUserProgressesGroupedByUserAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var allProgresses = await context.UserProgress
            .Include(up => up.Collection)
            .Where(up => up.IsActive)
            .ToListAsync(cancellationToken);

        var groupedProgresses = allProgresses
            .GroupBy(up => up.UserId)
            .OrderByDescending(g => g.Max(up => up.UpdatedAt ?? up.CreatedAt))
            .ToList();

        var totalCount = groupedProgresses.Count;
        var paginatedGroups = groupedProgresses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<IGrouping<string, UserProgress>>(paginatedGroups, totalCount, page, pageSize);
    }
}