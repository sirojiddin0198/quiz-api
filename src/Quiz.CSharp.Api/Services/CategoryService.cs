namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Services;

public sealed class CategoryService(
    ICSharpRepository repository,
    IMapper mapper) : ICategoryService
{
    public async Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await repository.GetCategoriesAsync(cancellationToken);
        return mapper.Map<List<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse?> GetCategoryByIdAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetCategoryByIdAsync(categoryId, cancellationToken);
        return category is not null ? mapper.Map<CategoryResponse>(category) : null;
    }
} 