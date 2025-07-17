namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class MCQOptionConfiguration : IEntityTypeConfiguration<MCQOption>
{
    public void Configure(EntityTypeBuilder<MCQOption> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(10);
        builder.Property(o => o.Option).HasMaxLength(1000);
        builder.Property(o => o.QuestionId).IsRequired();
        builder.Property(o => o.IsCorrect).IsRequired();
        
        builder.HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(o => o.QuestionId);
    }
} 