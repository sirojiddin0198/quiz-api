namespace Quiz.Shared.Authentication;

public interface ISubscriptionService
{
    Task<bool> HasAccessAsync(string feature, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetAvailableFeaturesAsync(CancellationToken cancellationToken = default);
    Task<UserSubscription?> GetUserSubscriptionAsync(CancellationToken cancellationToken = default);
}

public sealed record UserSubscription
{
    public required string UserId { get; init; }
    public required IReadOnlyList<string> Subscriptions { get; init; }
    public required DateTime? ExpiresAt { get; init; }
    public required bool IsActive { get; init; }
} 