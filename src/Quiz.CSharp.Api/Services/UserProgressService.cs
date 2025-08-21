namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.Shared.Authentication;
using Quiz.CSharp.Api.Services.Abstractions;

public sealed class UserProgressService(
    IUserProgressRepository userProgressRepository,
    ICollectionRepository collectionRepository,
    IMapper mapper,
    ICurrentUser currentUser) : IUserProgressService
{
    public async Task<List<CollectionProgressResponse>> GetUserProgressAsync(CancellationToken cancellationToken = default)
    {
        var responses = new List<CollectionProgressResponse>();
        if (currentUser.IsAuthenticated && currentUser.UserId is not null)
        {
            var collectionIds = await collectionRepository.GetAnsweredCollectionIdsByUserIdAsync(currentUser.UserId, cancellationToken);
            foreach (var collectionId in collectionIds)
            {
                var userProgress = await userProgressRepository.GetUserProgressOrDefaultAsync(currentUser.UserId, collectionId, cancellationToken);
                if (userProgress is not null)
                    responses.Add(mapper.Map<CollectionProgressResponse>(userProgress));
            }
        }
        
        return responses;
    }
} 