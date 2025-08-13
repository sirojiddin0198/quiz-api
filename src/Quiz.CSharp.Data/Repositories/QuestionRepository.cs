using Quiz.CSharp.Data.Data;
using Quiz.CSharp.Data.Repositories.Abstractions;

namespace Quiz.CSharp.Data.Repositories;

public sealed class QuestionRepository(ICSharpDbContext context) : IQuestionRepository
{
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
        => await context.Questions
            .Where(q => q.CollectionId == collectionId && q.IsActive)
            .Include(q => q.Collection)
            .OrderBy(q => q.Id)
            .Take(2)
            .ToListAsync(cancellationToken);

    public async Task<Question?> GetSingleOrDefaultAsync(int questionId, CancellationToken cancellationToken = default)
        => await context.Questions
            .Include(q => q.Collection)
            .FirstOrDefaultAsync(q => q.Id == questionId && q.IsActive, cancellationToken);
    
    public async Task<Question> CreateQuestionAsync(Question question, CancellationToken cancellationToken = default)
    {
        context.Questions.Add(question);
        await context.SaveChangesAsync(cancellationToken);
        return question;
    }
}