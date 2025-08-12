namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Services;
using Quiz.Shared.Authentication;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Api.Dtos;

public sealed class CollectionService(
    ICSharpRepository repository,
    IMapper mapper,
    ICurrentUser currentUser) : ICollectionService
{
    public async Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var collectionsWithCounts = await repository.GetCollectionsWithQuestionCountAsync(cancellationToken);
        var responses = new List<CollectionResponse>();

        foreach (var collectionWithCount in collectionsWithCounts)
        {
            var response = mapper.Map<CollectionResponse>(collectionWithCount.Collection);
            response = response with { TotalQuestions = collectionWithCount.QuestionCount };

            // Add user progress if authenticated
            if (currentUser.IsAuthenticated && currentUser.UserId is not null)
            {
                var userProgress = await repository.GetUserProgressAsync(
                    currentUser.UserId,
                    collectionWithCount.Collection.Id,
                    cancellationToken);

                if (userProgress is not null)
                {
                    response = response with
                    {
                        UserProgress = mapper.Map<UserProgressResponse>(userProgress)
                    };
                }
            }

            responses.Add(response);
        }

        return responses;
    }
     public async Task<CollectionResponse> CreateCollectionAsync(CreateCollection dto, CancellationToken cancellationToken = default)
    {
       var collection = mapper.Map<Collection>(dto);
        await repository.AddCollectionAsync(collection, cancellationToken);
        return mapper.Map<CollectionResponse>(collection);
    }
} 