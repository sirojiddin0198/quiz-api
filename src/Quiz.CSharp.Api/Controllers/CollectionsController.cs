namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Services;
using Quiz.Shared.Contracts;

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

    [HttpGet("{collectionId:int}")]
    [ProducesResponseType(typeof(ApiResponse<CollectionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CollectionResponse>), 404)]
    public async Task<IActionResult> GetCollection(int collectionId, CancellationToken cancellationToken)
    {
        var collection = await collectionService.GetCollectionByIdAsync(collectionId, cancellationToken);
        return collection is not null
            ? Ok(new ApiResponse<CollectionResponse>(collection))
            : NotFound(new ApiResponse<CollectionResponse>(Success: false, Message: "Collection not found"));
    }

    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<CollectionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CollectionResponse>), 404)]
    public async Task<IActionResult> GetCollectionByCode(string code, CancellationToken cancellationToken)
    {
        var collection = await collectionService.GetCollectionByCodeAsync(code, cancellationToken);
        return collection is not null
            ? Ok(new ApiResponse<CollectionResponse>(collection))
            : NotFound(new ApiResponse<CollectionResponse>(Success: false, Message: "Collection not found"));
    }
} 