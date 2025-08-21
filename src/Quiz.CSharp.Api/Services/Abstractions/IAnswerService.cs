namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.CSharp.Api.Contracts;

public interface IAnswerService
{
    Task<AnswerSubmissionResponse> SubmitAnswerAsync(
        int questionId,
        string answer,
        int timeSpentSeconds,
        CancellationToken cancellationToken = default);
    Task<UserAnswerResponse> GetLatestAnswerAsync(int questionId, CancellationToken cancellationToken = default);
} 