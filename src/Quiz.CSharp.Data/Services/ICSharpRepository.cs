namespace Quiz.CSharp.Data.Services;

using Quiz.CSharp.Data.Entities;
using Quiz.Shared.Common;

public interface ICSharpRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Question>> GetQuestionsByCategoryAsync(
        string categoryId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Question>> GetPreviewQuestionsAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<Question?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default);
    Task<UserAnswer?> GetLatestAnswerAsync(string userId, int questionId, CancellationToken cancellationToken = default);
    Task SaveAnswerAsync(UserAnswer answer, CancellationToken cancellationToken = default);
    Task<UserProgress?> GetUserProgressAsync(string userId, string categoryId, CancellationToken cancellationToken = default);
    Task UpdateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task<int> GetNextAttemptNumberAsync(string userId, int questionId, CancellationToken cancellationToken = default);
} 