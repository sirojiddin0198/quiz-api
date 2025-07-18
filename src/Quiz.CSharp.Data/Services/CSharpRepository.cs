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

    public async Task<Collection?> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default)
    {
        return await context.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.IsActive, cancellationToken);
    }

    public async Task<Collection?> GetCollectionByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await context.Collections
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive, cancellationToken);
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
            .FirstOrDefaultAsync(up => up.UserId == userId && up.CollectionId == collectionId, cancellationToken);
    }

    public async Task UpdateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default)
    {
        context.UserProgress.Update(progress);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetNextAttemptNumberAsync(string userId, int questionId, CancellationToken cancellationToken = default)
    {
        var lastAttempt = await context.UserAnswers
            .Where(ua => ua.UserId == userId && ua.QuestionId == questionId)
            .OrderByDescending(ua => ua.AttemptNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return (lastAttempt?.AttemptNumber ?? 0) + 1;
    }
}

public sealed class CollectionWithQuestionCount
{
    public required Collection Collection { get; init; }
    public int QuestionCount { get; init; }
} 