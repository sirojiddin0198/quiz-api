namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Services;

public sealed class CollectionService(
    ICSharpRepository repository,
    IMapper mapper) : ICollectionService
{
    public async Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var collections = await repository.GetCollectionsAsync(cancellationToken);
        return mapper.Map<List<CollectionResponse>>(collections);
    }

    public async Task<CollectionResponse?> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default)
    {
        var collection = await repository.GetCollectionByIdAsync(collectionId, cancellationToken);
        return collection is not null ? mapper.Map<CollectionResponse>(collection) : null;
    }

    public async Task<CollectionResponse?> GetCollectionByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var collection = await repository.GetCollectionByCodeAsync(code, cancellationToken);
        return collection is not null ? mapper.Map<CollectionResponse>(collection) : null;
    }
} 