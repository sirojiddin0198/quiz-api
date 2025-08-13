namespace Quiz.CSharp.Data.Repositories.Abstractions;

public interface ICollectionRepository
{
    Task<IReadOnlyList<Collection>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    Task<Collection> CreateCollectionAsync(Collection collection, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CollectionWithQuestionCount>> GetCollectionsWithQuestionCountAsync(CancellationToken cancellationToken = default);
    Task<bool> CollectionExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<List<int>> GetAnsweredCollectionIdsByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
}