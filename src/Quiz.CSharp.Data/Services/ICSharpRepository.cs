namespace Quiz.CSharp.Data.Services;

using Quiz.CSharp.Data.Entities;
using Quiz.Shared.Common;

public interface ICSharpRepository
{
    Task<IReadOnlyList<Collection>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    Task<Collection?> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default);
    Task<Collection?> GetCollectionByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Question>> GetQuestionsByCollectionAsync(
        int collectionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Question>> GetPreviewQuestionsAsync(int collectionId, CancellationToken cancellationToken = default);
    Task<Question?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default);
    Task<UserAnswer?> GetLatestAnswerAsync(string userId, int questionId, CancellationToken cancellationToken = default);
    Task SaveAnswerAsync(UserAnswer answer, CancellationToken cancellationToken = default);
    Task<UserProgress?> GetUserProgressAsync(string userId, int collectionId, CancellationToken cancellationToken = default);
    Task UpdateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task<int> GetNextAttemptNumberAsync(string userId, int questionId, CancellationToken cancellationToken = default);
} 