namespace Quiz.CSharp.Api.Validators;

using FluentValidation;
using Quiz.CSharp.Api.Contracts.Requests;

public sealed class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequest>
{
    public SubmitAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");

        RuleFor(x => x.Answer)
            .NotEmpty()
            .WithMessage("Answer cannot be empty")
            .MaximumLength(5000)
            .WithMessage("Answer cannot exceed 5000 characters");

        RuleFor(x => x.TimeSpentSeconds)
            .GreaterThan(0)
            .WithMessage("Time spent must be greater than 0");
    }
} 