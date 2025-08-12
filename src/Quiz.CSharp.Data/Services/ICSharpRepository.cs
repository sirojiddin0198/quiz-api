namespace Quiz.CSharp.Data.Services;

using Quiz.CSharp.Data.Entities;
using Quiz.Shared.Common;

public interface ICSharpRepository
{
    Task<IReadOnlyList<Collection>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CollectionWithQuestionCount>> GetCollectionsWithQuestionCountAsync(CancellationToken cancellationToken = default);
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
    Task CreateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task<(int totalQuestions, int answeredQuestions, int correctAnswers)> CalculateProgressStatsAsync(
        string userId,
        int collectionId,
        CancellationToken cancellationToken = default);
    Task<int> GetNextAttemptNumberAsync(string userId, int questionId, CancellationToken cancellationToken = default);

    // Management endpoints
    Task<PaginatedResult<UserProgress>> GetAllUserProgressesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<IGrouping<string, UserProgress>>> GetUserProgressesGroupedByUserAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Collection> CreateCollectionAsync(Collection collection, CancellationToken cancellationToken = default);
    Task<Question> CreateQuestionAsync(Question question, CancellationToken cancellationToken = default);
    Task<bool> CollectionExistsAsync(string code, CancellationToken cancellationToken = default);
    Task AddCollectionAsync(Collection collection, CancellationToken cancellationToken = default);
    Task<List<int>> GetAnsweredCollectionIdsByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
} 