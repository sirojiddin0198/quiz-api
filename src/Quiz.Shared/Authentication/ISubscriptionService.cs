namespace Quiz.Shared.Authentication;

public interface ISubscriptionService
{
    /// <summary>
    /// Checks if the current user has access to a specific feature
    /// </summary>
    /// <param name="feature">The feature to check access for</param>
    /// <returns>True if user has access, false otherwise</returns>
    Task<bool> HasAccessAsync(string feature, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all available features for the current user
    /// </summary>
    /// <returns>List of features the user has access to</returns>
    Task<IReadOnlyList<string>> GetAvailableFeaturesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the user's subscription details
    /// </summary>
    /// <returns>User's subscription information</returns>
    Task<UserSubscription?> GetUserSubscriptionAsync(CancellationToken cancellationToken = default);
}

public sealed record UserSubscription
{
    public required string UserId { get; init; }
    public required IReadOnlyList<string> Subscriptions { get; init; }
    public required DateTime? ExpiresAt { get; init; }
    public required bool IsActive { get; init; }
} 