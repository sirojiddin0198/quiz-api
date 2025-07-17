namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).ValueGeneratedOnAdd();
        builder.Property(q => q.Type).HasConversion<string>();
        builder.Property(q => q.CategoryId).HasMaxLength(100);
        builder.Property(q => q.Subcategory).HasMaxLength(200);
        builder.Property(q => q.Difficulty).HasMaxLength(50);
        builder.Property(q => q.Prompt).HasMaxLength(2000);
        
        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(q => q.UserAnswers)
            .WithOne(ua => ua.Question)
            .HasForeignKey(ua => ua.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(q => q.Hints)
            .WithOne(h => h.Question)
            .HasForeignKey(h => h.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(q => q.TestCases)
            .WithOne(tc => tc.Question)
            .HasForeignKey(tc => tc.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(q => q.CategoryId);
        builder.HasIndex(q => q.IsActive);
    }
} 