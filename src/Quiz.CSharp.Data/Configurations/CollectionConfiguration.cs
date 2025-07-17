namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        builder.ToTable("collections");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Code).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.Icon).HasMaxLength(50);
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.SortOrder);
        builder.HasIndex(c => c.Code).IsUnique();
    }
} 