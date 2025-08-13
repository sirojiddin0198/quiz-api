namespace Quiz.CSharp.Data.Repositories.Abstractions;

public interface IQuestionRepository
{
    Task<PaginatedResult<Question>> GetQuestionsByCollectionAsync(
        int collectionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Question>> GetPreviewQuestionsAsync(int collectionId, CancellationToken cancellationToken = default);
    Task<Question?> GetSingleOrDefaultAsync(int questionId, CancellationToken cancellationToken = default);
    Task<Question> CreateQuestionAsync(Question question, CancellationToken cancellationToken = default);
}