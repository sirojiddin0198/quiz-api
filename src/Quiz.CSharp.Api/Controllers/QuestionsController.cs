namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.Shared.Contracts;
using Quiz.Infrastructure.Authentication;
using Quiz.CSharp.Data.Models;

[ApiController]
[Route("api/csharp/questions")]
[Produces("application/json")]
[RequireSubscription("csharp-quiz")]
public sealed class QuestionsController(
    IQuestionService questionService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType<PaginatedApiResponse<QuestionResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuestions(
        [FromQuery] int collectionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var questions = await questionService.GetQuestionsByCollectionAsync(
            collectionId,
            page,
            pageSize,
            cancellationToken);
        return Ok(new PaginatedApiResponse<QuestionResponse>(
            questions.Items.ToList(),
            questions.TotalCount,
            questions.Page,
            questions.PageSize));
    }

    [HttpGet("preview")]
    [AllowAnonymous]
    [ProducesResponseType<ApiResponse<List<QuestionResponse>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreviewQuestions(
        [FromQuery] int collectionId,
        CancellationToken cancellationToken)
    {
        var questions = await questionService.GetPreviewQuestionsAsync(collectionId, cancellationToken);
        return Ok(new ApiResponse<List<QuestionResponse>>(questions));
    }

    [HttpPost("create")]
    [Authorize("Admin:Write")]
    [ProducesResponseType<ApiResponse<CreateQuestionResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateQuestion(
        [FromBody] CreateQuestionDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await questionService.CreateQuestionAsync(mapper.Map<CreateQuestionModel>(request), cancellationToken);
        return Ok(new ApiResponse<CreateQuestionResponse>(response.Value));
    }
} 