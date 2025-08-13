namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Contracts.Requests;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.Shared.Contracts;

[ApiController]
[Route("api/csharp/management/collections")]
[Produces("application/json")]
[Authorize(Policy = "Admin:Write")]
public sealed class CollectionManagementController(
    ICollectionService collectionService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ApiResponse<CreateCollectionResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiResponse<CreateCollectionResponse>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCollection(
        [FromBody] CreateCollectionRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await collectionService.CreateCollectionWithQuestionsAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(CreateCollection),
                new { id = result.Value!.Id },
                new ApiResponse<CreateCollectionResponse>(result.Value));
        }

        return BadRequest(new ApiResponse<CreateCollectionResponse>(
            Success: false,
            Message: result.ErrorMessage,
            Errors: result.Errors));
    }
} 