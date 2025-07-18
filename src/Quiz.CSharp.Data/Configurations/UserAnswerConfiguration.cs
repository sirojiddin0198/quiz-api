namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.ToTable("user_answers");
        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.Id).ValueGeneratedOnAdd();
        builder.Property(ua => ua.UserId).HasMaxLength(100);
        builder.Property(ua => ua.QuestionId).IsRequired();
        builder.Property(ua => ua.Answer).HasMaxLength(5000);
        builder.Property(ua => ua.IsCorrect);
        builder.Property(ua => ua.TimeSpentSeconds);
        builder.Property(ua => ua.SubmittedAt);
        builder.Property(ua => ua.AttemptNumber);
        
        builder.HasIndex(ua => ua.UserId);
        builder.HasIndex(ua => ua.QuestionId);
        builder.HasIndex(ua => new { ua.UserId, ua.QuestionId });
        
        builder.HasOne(ua => ua.Question)
            .WithMany(q => q.UserAnswers)
            .HasForeignKey(ua => ua.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 