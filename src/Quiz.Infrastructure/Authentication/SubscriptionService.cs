namespace Quiz.Infrastructure.Authentication;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Quiz.Shared.Authentication;

public sealed class SubscriptionService(
    ICurrentUser currentUser,
    IHttpContextAccessor httpContextAccessor,
    ILogger<SubscriptionService> logger) : ISubscriptionService
{
    private const string SubscriptionClaimType = "ustoz-membership";
    
    private static readonly Dictionary<string, HashSet<string>> FeatureAccessMap = new()
    {
        ["csharp-quiz"] = ["csharp-quiz"],
        ["premium"] = ["csharp-quiz", "advanced-features", "unlimited-questions"],
        ["basic"] = ["csharp-quiz"],
        ["admin"] = ["csharp-quiz", "advanced-features", "unlimited-questions", "admin-panel"]
    };

    public Task<bool> HasAccessAsync(string feature, CancellationToken cancellationToken = default)
    {
        if (currentUser.IsAuthenticated is false)
        {
            logger.LogDebug("User is not authenticated, denying access to feature: {Feature}", feature);
            return Task.FromResult(false);
        }

        var userSubscription = GetUserSubscriptionAsync(cancellationToken).Result;
        if (userSubscription == null)
        {
            logger.LogDebug("No subscription found for user {UserId}, denying access to feature: {Feature}", 
                currentUser.UserId, feature);
            return Task.FromResult(false);
        }

        if (userSubscription.IsActive is false)
        {
            logger.LogDebug("Subscription is not active for user {UserId}, denying access to feature: {Feature}", 
                currentUser.UserId, feature);
            return Task.FromResult(false);
        }

        foreach (var subscription in userSubscription.Subscriptions)
        {
            if (FeatureAccessMap.TryGetValue(subscription, out var grantedFeatures)
                && grantedFeatures.Contains(feature))
            {
                logger.LogDebug("User {UserId} has access to feature {Feature} via subscription {Subscription}", 
                    currentUser.UserId, feature, subscription);
                return Task.FromResult(true);
            }
        }

        logger.LogDebug("User {UserId} does not have access to feature {Feature}. User subscriptions: {Subscriptions}", 
            currentUser.UserId, feature, string.Join(", ", userSubscription.Subscriptions));
        return Task.FromResult(false);
    }

    public async Task<IReadOnlyList<string>> GetAvailableFeaturesAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.IsAuthenticated is false) return [];

        var userSubscription = await GetUserSubscriptionAsync(cancellationToken);
        if (userSubscription?.IsActive != true) return [];

        var availableFeatures = new HashSet<string>();
        
        foreach (var subscription in userSubscription.Subscriptions)
            if (FeatureAccessMap.TryGetValue(subscription, out var grantedFeatures))
                foreach (var feature in grantedFeatures)
                    availableFeatures.Add(feature);

        return availableFeatures.ToList();
    }

    public Task<UserSubscription?> GetUserSubscriptionAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.IsAuthenticated is false || string.IsNullOrEmpty(currentUser.UserId))
            return Task.FromResult<UserSubscription?>(null);

        var subscriptionClaims = ExtractSubscriptionClaims();
        
        if (subscriptionClaims.Any() is false)
        {
            logger.LogWarning("No subscription claims found for user {UserId}", currentUser.UserId);
            return Task.FromResult<UserSubscription?>(null);
        }

        var expiresAt = ExtractExpirationClaim();

        var subscription = new UserSubscription
        {
            UserId = currentUser.UserId,
            Subscriptions = subscriptionClaims.ToList(),
            ExpiresAt = expiresAt,
            IsActive = expiresAt == null || expiresAt > DateTime.UtcNow
        };

        return Task.FromResult<UserSubscription?>(subscription);
    }

    private IEnumerable<string> ExtractSubscriptionClaims()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User == null) return [];

        var subscriptionClaims = httpContext.User.Claims.Where(claim => claim.Type == SubscriptionClaimType);
        if (subscriptionClaims?.Any() is not true) return [];

        return subscriptionClaims.Select(claim => claim.Value);
    }

    private DateTime? ExtractExpirationClaim()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User == null) return null;

        var expClaim = httpContext.User.FindFirst("exp");
        if (expClaim == null || !long.TryParse(expClaim.Value, out var exp)) return null;

        return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
    }
} 