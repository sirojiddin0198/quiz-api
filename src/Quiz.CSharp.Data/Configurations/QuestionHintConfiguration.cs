namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class QuestionHintConfiguration : IEntityTypeConfiguration<QuestionHint>
{
    public void Configure(EntityTypeBuilder<QuestionHint> builder)
    {
        builder.HasKey(qh => qh.Id);
        builder.Property(qh => qh.Id).ValueGeneratedOnAdd();
        builder.Property(qh => qh.QuestionId).IsRequired();
        builder.Property(qh => qh.Hint).HasMaxLength(1000).IsRequired();
        builder.Property(qh => qh.OrderIndex).IsRequired();
        
        builder.HasOne(qh => qh.Question)
            .WithMany(q => q.Hints)
            .HasForeignKey(qh => qh.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(qh => qh.QuestionId);
    }
} 