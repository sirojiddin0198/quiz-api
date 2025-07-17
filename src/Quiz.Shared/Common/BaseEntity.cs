namespace Quiz.Shared.Common;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; init; } = true;
} 