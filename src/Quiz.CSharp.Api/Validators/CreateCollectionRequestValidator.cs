namespace Quiz.CSharp.Api.Validators;

using FluentValidation;
using Quiz.CSharp.Api.Contracts.Requests;

public sealed class CreateCollectionRequestValidator : AbstractValidator<CreateCollectionRequest>
{
    public CreateCollectionRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Collection code is required")
            .MaximumLength(50)
            .WithMessage("Collection code must not exceed 50 characters")
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Collection code must contain only lowercase letters, numbers, and hyphens");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Collection title is required")
            .MaximumLength(200)
            .WithMessage("Collection title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Collection description is required")
            .MaximumLength(1000)
            .WithMessage("Collection description must not exceed 1000 characters");

        RuleFor(x => x.Icon)
            .NotEmpty()
            .WithMessage("Collection icon is required")
            .MaximumLength(50)
            .WithMessage("Collection icon must not exceed 50 characters");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sort order must be greater than or equal to 0");

        RuleFor(x => x.Questions)
            .NotEmpty()
            .WithMessage("At least one question is required");

        RuleForEach(x => x.Questions)
            .SetValidator(new CreateQuestionRequestValidator());
    }
}

public sealed class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    private static readonly string[] ValidQuestionTypes = 
    {
        "mcq", "true_false", "fill", "error_spotting", "output_prediction", "code_writing"
    };

    private static readonly string[] ValidDifficulties = 
    {
        "Beginner", "Intermediate", "Advanced"
    };

    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Question type is required")
            .Must(type => ValidQuestionTypes.Contains(type))
            .WithMessage($"Question type must be one of: {string.Join(", ", ValidQuestionTypes)}");

        RuleFor(x => x.Subcategory)
            .NotEmpty()
            .WithMessage("Question subcategory is required")
            .MaximumLength(200)
            .WithMessage("Question subcategory must not exceed 200 characters");

        RuleFor(x => x.Difficulty)
            .NotEmpty()
            .WithMessage("Question difficulty is required")
            .Must(difficulty => ValidDifficulties.Contains(difficulty))
            .WithMessage($"Question difficulty must be one of: {string.Join(", ", ValidDifficulties)}");

        RuleFor(x => x.Prompt)
            .NotEmpty()
            .WithMessage("Question prompt is required")
            .MaximumLength(2000)
            .WithMessage("Question prompt must not exceed 2000 characters");

        RuleFor(x => x.EstimatedTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Estimated time must be greater than 0")
            .LessThanOrEqualTo(120)
            .WithMessage("Estimated time must not exceed 120 minutes");

        RuleFor(x => x.Metadata)
            .NotEmpty()
            .WithMessage("Question metadata is required")
            .Must(BeValidJson)
            .WithMessage("Question metadata must be valid JSON");
    }

    private static bool BeValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
} 