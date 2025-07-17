namespace Quiz.Shared.Authentication;

public interface ICurrentUser
{
    string? UserId { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
} 