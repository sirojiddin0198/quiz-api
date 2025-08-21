namespace Quiz.CSharp.Api.Services;

using Quiz.Shared.Authentication;
using Quiz.Shared.Common;
using Quiz.CSharp.Api.Services.Abstractions;

public sealed class SubscriptionGuard(ISubscriptionService subscriptionService) : ISubscriptionGuard
{
    public async Task<SimpleResult> EnsureAccessAsync(string feature, CancellationToken cancellationToken = default)
    {
        var hasAccess = await subscriptionService.HasAccessAsync(feature, cancellationToken);
        
        return hasAccess 
            ? SimpleResult.Success() 
            : SimpleResult.Failure($"Access denied. Subscription required for feature: {feature}");
    }

    public async Task<bool> HasAccessAsync(string feature, CancellationToken cancellationToken = default)
        => await subscriptionService.HasAccessAsync(feature, cancellationToken);
} 