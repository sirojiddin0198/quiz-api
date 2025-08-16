namespace Quiz.CSharp.Api.Validators;

using FluentValidation;
using Quiz.CSharp.Api.Dtos;



public sealed class CreateCollectionValidator : AbstractValidator<CreateCollection>
{
    public CreateCollectionValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-z0-9-]+$");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Icon)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
    
}