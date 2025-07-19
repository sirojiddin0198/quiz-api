namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(EntityTypeBuilder<UserProgress> builder)
    {
        builder.ToTable("user_progress");
        builder.HasKey(up => new { up.UserId, up.CollectionId });
        builder.Property(up => up.UserId).HasMaxLength(100);
        builder.Property(up => up.Username).HasMaxLength(100);
        builder.Property(up => up.Name).HasMaxLength(200);
        builder.Property(up => up.TelegramUsername).HasMaxLength(100);
        builder.Property(up => up.CollectionId).IsRequired();
        builder.Property(up => up.TotalQuestions);
        builder.Property(up => up.AnsweredQuestions);
        builder.Property(up => up.CorrectAnswers);
        builder.Property(up => up.SuccessRate).HasPrecision(5, 2);
        builder.Property(up => up.LastAnsweredAt);
        
        builder.HasIndex(up => up.UserId);
        builder.HasIndex(up => up.CollectionId);
        
        builder.HasOne(up => up.Collection)
            .WithMany(c => c.UserProgress)
            .HasForeignKey(up => up.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 