namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Contracts.Requests;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.Shared.Contracts;
using Quiz.Infrastructure.Authentication;

[ApiController]
[Route("api/csharp/answers")]
[Produces("application/json")]
[RequireSubscription("csharp-quiz")]
public sealed class AnswersController(IAnswerService answerService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType<ApiResponse<AnswerSubmissionResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitAnswer(
        [FromBody] SubmitAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await answerService.SubmitAnswerAsync(
            request.QuestionId,
            request.Answer,
            request.TimeSpentSeconds,
            cancellationToken);

        return Ok(new ApiResponse<AnswerSubmissionResponse>(result));
    }

    [HttpGet("{questionId:int}/latest")]
    [Authorize]
    [ProducesResponseType<ApiResponse<UserAnswerResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLatestAnswer(int questionId, CancellationToken cancellationToken)
    {
        var answer = await answerService.GetLatestAnswerAsync(questionId, cancellationToken);
        return Ok(new ApiResponse<UserAnswerResponse>(answer));
    }
}