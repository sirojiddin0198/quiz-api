namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> builder)
    {
        builder.HasKey(tc => tc.Id);
        builder.Property(tc => tc.Id).ValueGeneratedOnAdd();
        builder.Property(tc => tc.QuestionId).IsRequired();
        builder.Property(tc => tc.Input).HasMaxLength(2000).IsRequired();
        builder.Property(tc => tc.ExpectedOutput).HasMaxLength(2000).IsRequired();
        
        builder.HasOne(tc => tc.Question)
            .WithMany(q => q.TestCases)
            .HasForeignKey(tc => tc.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(tc => tc.QuestionId);
    }
} 