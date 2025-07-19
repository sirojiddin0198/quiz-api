namespace Quiz.Shared.Authentication;

public interface ICurrentUser
{
    string? UserId { get; }
    string? Username { get; }
    string? Name { get; }
    string? TelegramUsername { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> RealmRoles { get; }
    IReadOnlyList<string> ResourceRoles { get; }
    bool HasResourceRole(string role);
    bool HasRealmRole(string role);
} 