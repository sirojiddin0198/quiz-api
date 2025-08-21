namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.CSharp.Api.Contracts;

public interface ICollectionService
{
    Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    Task<CreateCollectionResponse> CreateCollectionWithQuestionsAsync(
        CreateCollectionRequest request, 
        CancellationToken cancellationToken = default);
} 