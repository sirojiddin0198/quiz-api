namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(EntityTypeBuilder<UserProgress> builder)
    {
        builder.HasKey(up => new { up.UserId, up.CollectionId });
        builder.Property(up => up.UserId).HasMaxLength(100);
        builder.Property(up => up.CollectionId).IsRequired();
        
        builder.Property(up => up.SuccessRate).HasPrecision(5, 2);
        
        builder.HasIndex(up => up.UserId);
        builder.HasIndex(up => up.CollectionId);
    }
} 