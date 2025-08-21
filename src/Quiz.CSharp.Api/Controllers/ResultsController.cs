namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts.Reviews;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.Shared.Contracts;
using Quiz.Infrastructure.Authentication;
using Quiz.Infrastructure.Exceptions;

[ApiController]
[Route("api/csharp/results")]
[Produces("application/json")]
[RequireSubscription("csharp-quiz")]
public sealed class ResultsController(IResultsService resultsService) : ControllerBase
{
    [HttpGet("collections/{collectionId}/review")]
    [Authorize]
    [ProducesResponseType<ApiResponse<List<QuestionReviewResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<List<QuestionReviewResponse>>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAnswerReview(
        int collectionId,
        [FromQuery] bool includeUnanswered = false,
        CancellationToken cancellationToken = default)
    {
        var result = await resultsService.GetAnswerReviewAsync(collectionId, includeUnanswered, cancellationToken);
        return Ok(new ApiResponse<List<QuestionReviewResponse>>(result));
    }

    [HttpPost("sessions/{sessionId}/complete")]
    [AllowAnonymous]
    [ProducesResponseType<ApiResponse<SessionResultsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<SessionResultsResponse>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteSession(
        string sessionId,
        [FromBody] CompleteSessionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await resultsService.CompleteSessionAsync(sessionId, request, cancellationToken);
        return Ok(new ApiResponse<SessionResultsResponse>(result));
    }
} 