namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.CSharp.Api.Contracts;

public interface ICollectionService
{
    Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    Task<Result<CreateCollectionResponse>> CreateCollectionWithQuestionsAsync(
        CreateCollectionRequest request, 
        CancellationToken cancellationToken = default);
} 