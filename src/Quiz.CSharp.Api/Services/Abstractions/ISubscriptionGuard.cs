namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.Shared.Common;

public interface ISubscriptionGuard
{
    /// <summary>
    /// Ensures the current user has access to the specified feature
    /// </summary>
    /// <param name="feature">The feature to check access for</param>
    /// <returns>Success result if access is granted, failure result otherwise</returns>
    Task<SimpleResult> EnsureAccessAsync(string feature, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if the current user has access to the specified feature
    /// </summary>
    /// <param name="feature">The feature to check access for</param>
    /// <returns>True if access is granted, false otherwise</returns>
    Task<bool> HasAccessAsync(string feature, CancellationToken cancellationToken = default);
} 