namespace Quiz.Shared.Common;

public readonly record struct Result<T>
{
    public T? Value { get; init; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<string> Errors { get; init; }

    public Result()
    {
        Value = default;
        IsSuccess = false;
        ErrorMessage = null;
        Errors = [];
    }

    public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static Result<T> Failure(IReadOnlyList<string> errors) => new() { IsSuccess = false, Errors = errors };
} 