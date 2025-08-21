namespace Quiz.CSharp.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz.CSharp.Data.Entities;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("questions");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).ValueGeneratedOnAdd();
        builder.Property(q => q.CollectionId).IsRequired();
        builder.Property(q => q.Subcategory).HasMaxLength(200);
        builder.Property(q => q.Difficulty).HasMaxLength(50);
        builder.Property(q => q.Prompt).HasMaxLength(2000);
        builder.Property(q => q.EstimatedTimeMinutes);
        
        builder.Property(q => q.Metadata)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasOne(q => q.Collection)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasDiscriminator<string>("question_type")
            .HasValue<MCQQuestion>("mcq")
            .HasValue<TrueFalseQuestion>("true_false")
            .HasValue<FillQuestion>("fill")
            .HasValue<ErrorSpottingQuestion>("error_spotting")
            .HasValue<OutputPredictionQuestion>("output_prediction")
            .HasValue<CodeWritingQuestion>("code_writing");
    }
} 