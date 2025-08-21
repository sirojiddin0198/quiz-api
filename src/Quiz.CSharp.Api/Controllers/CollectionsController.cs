namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Services;
using Quiz.Shared.Contracts;
using Quiz.CSharp.Api.Dtos;

[ApiController]
[Route("api/csharp/collections")]
[Produces("application/json")]
public sealed class CollectionsController(ICollectionService collectionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CollectionResponse>>), 200)]
    public async Task<IActionResult> GetCollections(CancellationToken cancellationToken)
    {
        var collections = await collectionService.GetCollectionsAsync(cancellationToken);
        return Ok(new ApiResponse<List<CollectionResponse>>(collections));
    }

    [HttpPost]
    [Authorize(Policy = "Admin:Write")]
    [ProducesResponseType(typeof(ApiResponse<CollectionResponse>), 201)]
    public async Task<IActionResult> CreateCollection([FromBody] CreateCollection dto, CancellationToken cancellationToken)
{
    var result = await collectionService.CreateCollectionAsync(dto, cancellationToken);

    if (!result.IsSuccess)
    {
        return BadRequest(result.ErrorMessage);
    }

    return CreatedAtAction(nameof(GetCollections),new { id = result.Value!.Id },result.Value);
}
} 