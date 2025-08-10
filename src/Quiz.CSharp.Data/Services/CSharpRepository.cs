namespace Quiz.CSharp.Data.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.CSharp.Data.Entities;
using Quiz.Shared.Common;

public sealed class CSharpRepository(ICSharpDbContext context) : ICSharpRepository
{
    public async Task<IReadOnlyList<Collection>> GetCollectionsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Collections
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CollectionWithQuestionCount>> GetCollectionsWithQuestionCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Collections
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CollectionWithQuestionCount
            {
                Collection = c,
                QuestionCount = context.Questions.Count(q => q.CollectionId == c.Id && q.IsActive)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Question>> GetQuestionsByCollectionAsync(
        int collectionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Questions
            .Where(q => q.CollectionId == collectionId && q.IsActive)
            .Include(q => q.Collection)
            .OrderBy(q => q.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Question>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<Question>> GetPreviewQuestionsAsync(int collectionId, CancellationToken cancellationToken = default)
    {
        return await context.Questions
            .Where(q => q.CollectionId == collectionId && q.IsActive)
            .Include(q => q.Collection)
            .OrderBy(q => q.Id)
            .Take(2)
            .ToListAsync(cancellationToken);
    }

    public async Task<Question?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await context.Questions
            .Include(q => q.Collection)
            .FirstOrDefaultAsync(q => q.Id == questionId && q.IsActive, cancellationToken);
    }

    public async Task<UserAnswer?> GetLatestAnswerAsync(string userId, int questionId, CancellationToken cancellationToken = default)
    {
        return await context.UserAnswers
            .Where(ua => ua.UserId == userId && ua.QuestionId == questionId)
            .OrderByDescending(ua => ua.SubmittedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveAnswerAsync(UserAnswer answer, CancellationToken cancellationToken = default)
    {
        context.UserAnswers.Add(answer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserProgress?> GetUserProgressAsync(string userId, int collectionId, CancellationToken cancellationToken = default)
    {
        return await context.UserProgress
            .Include(up => up.Collection)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.CollectionId == collectionId, cancellationToken);
    }

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
        // Get total questions in collection
        var totalQuestions = await context.Questions
            .Where(q => q.CollectionId == collectionId && q.IsActive)
            .CountAsync(cancellationToken);

        // Get distinct answered questions and count correct answers
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

    public async Task<int> GetNextAttemptNumberAsync(string userId, int questionId, CancellationToken cancellationToken = default)
    {
        var lastAttempt = await context.UserAnswers
            .Where(ua => ua.UserId == userId && ua.QuestionId == questionId)
            .OrderByDescending(ua => ua.AttemptNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return (lastAttempt?.AttemptNumber ?? 0) + 1;
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
        // Get all user progresses with collection data
        var allProgresses = await context.UserProgress
            .Include(up => up.Collection)
            .Where(up => up.IsActive)
            .ToListAsync(cancellationToken);

        // Group by user and order by most recent activity
        var groupedProgresses = allProgresses
            .GroupBy(up => up.UserId)
            .OrderByDescending(g => g.Max(up => up.UpdatedAt ?? up.CreatedAt))
            .ToList();

        var totalCount = groupedProgresses.Count;

        // Apply pagination to grouped results
        var paginatedGroups = groupedProgresses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<IGrouping<string, UserProgress>>(paginatedGroups, totalCount, page, pageSize);
    }

    public async Task<Collection> CreateCollectionAsync(Collection collection, CancellationToken cancellationToken = default)
    {
        context.Collections.Add(collection);
        await context.SaveChangesAsync(cancellationToken);
        return collection;
    }

    public async Task<Question> CreateQuestionAsync(Question question, CancellationToken cancellationToken = default)
    {
        context.Questions.Add(question);
        await context.SaveChangesAsync(cancellationToken);
        return question;
    }

    public async Task<bool> CollectionExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await context.Collections
            .AnyAsync(c => c.Code == code && c.IsActive, cancellationToken);
    }
    
    public async Task<List<int>> GetAnsweredCollectionIdsByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await context.UserAnswers
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.Question.CollectionId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}

public sealed class CollectionWithQuestionCount
{
    public required Collection Collection { get; init; }
    public int QuestionCount { get; init; }
} 