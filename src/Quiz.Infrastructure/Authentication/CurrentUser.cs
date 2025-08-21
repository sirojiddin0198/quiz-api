namespace Quiz.Infrastructure.Authentication;

using Quiz.Shared.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly ClaimsPrincipal? _principal = httpContextAccessor.HttpContext?.User;

    public string? UserId => _principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? Username => _principal?.FindFirst("preferred_username")?.Value;
    public string? Name => _principal?.FindFirst("name")?.Value;
    public string? TelegramUsername => _principal?.FindFirst("telegram_username")?.Value;
    public string? Email => _principal?.FindFirst(ClaimTypes.Email)?.Value;
    public string? FirstName => _principal?.FindFirst(ClaimTypes.GivenName)?.Value;
    public string? LastName => _principal?.FindFirst(ClaimTypes.Surname)?.Value;
    public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;
    
    public IReadOnlyList<string> Roles => _principal?.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList() ?? [];

    public IReadOnlyList<string> RealmRoles
    {
        get
        {
            var realmAccessClaim = _principal?.FindFirst("realm_access")?.Value;
            if (string.IsNullOrEmpty(realmAccessClaim)) return [];

            try
            {
                var realmAccess = JsonSerializer.Deserialize<RealmAccess>(realmAccessClaim);
                return realmAccess?.Roles ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    public IReadOnlyList<string> ResourceRoles
    {
        get
        {
            var resourceAccessClaim = _principal?.FindFirst("resource_access")?.Value;
            if (string.IsNullOrEmpty(resourceAccessClaim)) return [];

            try
            {
                var resourceAccess = JsonSerializer.Deserialize<Dictionary<string, ResourceAccess>>(resourceAccessClaim);
                var clientRoles = new List<string>();
                
                if (resourceAccess is not null)
                {
                    foreach (var resource in resourceAccess.Values)
                    {
                        if (resource.Roles is not null)
                            clientRoles.AddRange(resource.Roles);
                    }
                }
                
                return clientRoles;
            }
            catch
            {
                return [];
            }
        }
    }

    public bool HasResourceRole(string role) => ResourceRoles.Contains(role);
    public bool HasRealmRole(string role) => RealmRoles.Contains(role);

    private class RealmAccess
    {
        public List<string>? Roles { get; set; }
    }

    private class ResourceAccess
    {
        public List<string>? Roles { get; set; }
    }
} 