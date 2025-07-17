namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Api.Contracts;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetCategoryByIdAsync(string categoryId, CancellationToken cancellationToken = default);
} 