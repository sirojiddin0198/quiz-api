namespace Quiz.Shared.Contracts;

public record ApiResponse<T>(
    T? Data = default,
    bool Success = true,
    string? Message = null,
    IReadOnlyList<string>? Errors = null);

public sealed record PaginatedApiResponse<T>(
    IReadOnlyList<T> Data,
    int TotalCount,
    int Page,
    int PageSize,
    bool Success = true,
    string? Message = null) : ApiResponse<IReadOnlyList<T>>(Data, Success, Message); 