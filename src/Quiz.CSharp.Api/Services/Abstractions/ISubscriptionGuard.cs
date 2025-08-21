namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.Shared.Common;

public interface ISubscriptionGuard
{
    Task<SimpleResult> EnsureAccessAsync(string feature, CancellationToken cancellationToken = default);
    Task<bool> HasAccessAsync(string feature, CancellationToken cancellationToken = default);
} 