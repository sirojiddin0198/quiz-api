namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Services;
using Quiz.Shared.Authentication;

public sealed class UserProgressService(
    ICSharpRepository repository,
    IMapper mapper,
    ICurrentUser currentUser) : IUserProgressService
{
    public async Task<List<CollectionProgressResponse>> GetUserProgressAsync(CancellationToken cancellationToken = default)
    {
        var responses = new List<CollectionProgressResponse>();
        if (currentUser.IsAuthenticated && currentUser.UserId is not null)
        {
            var collectionIds = await repository.GetAnsweredCollectionIdsByUserIdAsync(currentUser.UserId, cancellationToken);
            foreach (var collectionId in collectionIds)
            {
                var userProgress = await repository.GetUserProgressAsync(currentUser.UserId, collectionId, cancellationToken);
                if (userProgress is not null)
                    responses.Add(mapper.Map<CollectionProgressResponse>(userProgress));
            }
        }
        return responses;
    }
} 