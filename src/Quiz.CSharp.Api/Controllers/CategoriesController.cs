namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Services;
using Quiz.Shared.Contracts;

[ApiController]
[Route("api/csharp/categories")]
[Produces("application/json")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ApiResponse<List<CategoryResponse>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetCategoriesAsync(cancellationToken);
        return Ok(new ApiResponse<List<CategoryResponse>>(categories));
    }

    [HttpGet("{categoryId}")]
    [ProducesResponseType<ApiResponse<CategoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<CategoryResponse>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(string categoryId, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetCategoryByIdAsync(categoryId, cancellationToken);
        return category is not null
            ? Ok(new ApiResponse<CategoryResponse>(category))
            : NotFound(new ApiResponse<CategoryResponse>(Success: false, Message: "Category not found"));
    }
} 