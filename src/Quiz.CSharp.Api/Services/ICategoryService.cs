namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Api.Contracts;

public interface ICollectionService
{
    Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    Task<CollectionResponse?> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default);
    Task<CollectionResponse?> GetCollectionByCodeAsync(string code, CancellationToken cancellationToken = default);
} 