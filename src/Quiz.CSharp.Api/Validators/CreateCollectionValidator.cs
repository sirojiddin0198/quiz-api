namespace Quiz.CSharp.Api.Validators;

using FluentValidation;
using Quiz.CSharp.Api.Dtos;



public sealed class CreateCollectionValidator : AbstractValidator<CreateCollection>
{
    public CreateCollectionValidator()
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
    }
    
}