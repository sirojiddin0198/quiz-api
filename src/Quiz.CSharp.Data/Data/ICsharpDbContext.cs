namespace Quiz.CSharp.Data.Data;

public interface ICSharpDbContext
{
    DbSet<Collection> Collections { get; set; }
    DbSet<Question> Questions { get; set; }
    DbSet<UserAnswer> UserAnswers { get; set; }
    DbSet<UserProgress> UserProgress { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
