namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.CSharp.Api.Contracts;
using Quiz.Shared.Common;

public interface IAnswerService
{
    Task<Result<AnswerSubmissionResponse>> SubmitAnswerAsync(
        int questionId,
        string answer,
        int timeSpentSeconds,
        CancellationToken cancellationToken = default);
    Task<UserAnswerResponse?> GetLatestAnswerOrDefaultAsync(int questionId, CancellationToken cancellationToken = default);
} 