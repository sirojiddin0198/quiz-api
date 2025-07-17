namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.Id).ValueGeneratedOnAdd();
        builder.Property(ua => ua.UserId).HasMaxLength(100);
        builder.Property(ua => ua.Answer).HasMaxLength(5000);
        
        builder.HasIndex(ua => ua.UserId);
        builder.HasIndex(ua => ua.QuestionId);
        builder.HasIndex(ua => new { ua.UserId, ua.QuestionId });
    }
} 