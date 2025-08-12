namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Dtos;

public interface ICollectionService
{
    Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    Task<CollectionResponse> CreateCollectionAsync(CreateCollection dto, CancellationToken cancellationToken = default);
} 