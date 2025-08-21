namespace Quiz.CSharp.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.Shared.Contracts;

[ApiController]
[Route("api/csharp/management/user-progresses")]
[Produces("application/json")]
[Authorize(Policy = "Admin:Read")]
public sealed class UserProgressManagementController(
    IUserProgressRepository userProgressRepository,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PaginatedApiResponse<UserProgressGroupedResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUserProgresses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var groupedProgresses = await userProgressRepository.GetUserProgressesGroupedByUserAsync(
            page,
            pageSize,
            cancellationToken);
        
        var responses = new List<UserProgressGroupedResponse>();

        foreach (var userGroup in groupedProgresses.Items)
        {
            var userProgresses = userGroup.ToList();
            var firstProgress = userProgresses.First();

            var totalQuestionsAnswered = userProgresses.Sum(up => up.AnsweredQuestions);
            var totalCorrectAnswers = userProgresses.Sum(up => up.CorrectAnswers);
            var overallSuccessRate = totalQuestionsAnswered > 0 
                ? Math.Round((decimal)totalCorrectAnswers / totalQuestionsAnswered * 100, 2) 
                : 0;

            var response = new UserProgressGroupedResponse
            {
                UserId = userGroup.Key,
                Username = firstProgress.Username,
                Name = firstProgress.Name,
                TelegramUsername = firstProgress.TelegramUsername,
                FirstActivityAt = userProgresses.Min(up => up.CreatedAt),
                LastActivityAt = userProgresses.Max(up => up.UpdatedAt ?? up.CreatedAt),
                TotalCollections = userProgresses.Count,
                TotalQuestionsAnswered = totalQuestionsAnswered,
                TotalCorrectAnswers = totalCorrectAnswers,
                OverallSuccessRate = overallSuccessRate,
                CollectionProgresses = mapper.Map<List<CollectionProgressResponse>>(userProgresses
                    .OrderByDescending(up => up.UpdatedAt ?? up.CreatedAt))
            };

            responses.Add(response);
        }

        return Ok(new PaginatedApiResponse<UserProgressGroupedResponse>(
            responses,
            groupedProgresses.TotalCount,
            groupedProgresses.Page,
            groupedProgresses.PageSize));
    }
} 