namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Contracts.Requests;
using Quiz.Shared.Common;

public interface ICollectionManagementService
{
    Task<Result<CreateCollectionResponse>> CreateCollectionWithQuestionsAsync(
        CreateCollectionRequest request, 
        CancellationToken cancellationToken = default);
} 