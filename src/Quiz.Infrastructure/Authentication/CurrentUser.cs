namespace Quiz.Infrastructure.Authentication;

using Quiz.Shared.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly ClaimsPrincipal? _principal = httpContextAccessor.HttpContext?.User;

    public string? UserId => _principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? Email => _principal?.FindFirst(ClaimTypes.Email)?.Value;
    public string? FirstName => _principal?.FindFirst(ClaimTypes.GivenName)?.Value;
    public string? LastName => _principal?.FindFirst(ClaimTypes.Surname)?.Value;
    public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;
    public IReadOnlyList<string> Roles => _principal?.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList() ?? [];
} 