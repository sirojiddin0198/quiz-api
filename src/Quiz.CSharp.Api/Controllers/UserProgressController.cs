namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts;
using Quiz.Shared.Contracts;
using Quiz.Infrastructure.Authentication;

[ApiController]
[Route("api/csharp/user-progress")]
[Produces("application/json")]
[RequireSubscription("csharp-quiz")]
public sealed class UserProgressController(IUserProgressService userProgressService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<UserProgressResponse>), 200)]
    public async Task<IActionResult> GetUserProgress(CancellationToken cancellationToken)
    {
        var userProgress = await userProgressService.GetUserProgressAsync(cancellationToken);
        return Ok(new ApiResponse<List<CollectionProgressResponse>>(userProgress));
    }
}