namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.CSharp.Api.Contracts.Reviews;
using Quiz.Shared.Common;

public interface IResultsService
{
    Task<Result<List<QuestionReviewResponse>>> GetAnswerReviewAsync(
        int collectionId,
        bool includeUnanswered = false,
        CancellationToken cancellationToken = default);
    
    Task<Result<SessionResultsResponse>> CompleteSessionAsync(
        string sessionId,
        CompleteSessionRequest request,
        CancellationToken cancellationToken = default);
} 