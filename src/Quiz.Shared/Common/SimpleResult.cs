namespace Quiz.Shared.Common;

public readonly record struct SimpleResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public SimpleResult()
    {
        IsSuccess = false;
        ErrorMessage = null;
    }

    public static SimpleResult Success() => new() { IsSuccess = true };
    public static SimpleResult Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
} 