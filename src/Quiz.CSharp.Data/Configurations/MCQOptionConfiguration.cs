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
        
        builder.HasIndex(o => o.QuestionId);
    }
} 