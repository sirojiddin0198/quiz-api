namespace Quiz.CSharp.Api.Validators;

using FluentValidation;
using Quiz.CSharp.Api.Contracts.Reviews;

public sealed class CompleteSessionRequestValidator : AbstractValidator<CompleteSessionRequest>
{
    public CompleteSessionRequestValidator()
    {
        RuleFor(x => x.Answers)
            .NotNull().WithMessage("Answers are required")
            .NotEmpty().WithMessage("At least one answer must be provided");

        RuleForEach(x => x.Answers)
            .ChildRules(answer =>
            {
                answer.RuleFor(a => a.QuestionId)
                    .GreaterThan(0).WithMessage("Question ID must be greater than 0");

                answer.RuleFor(a => a.Answer)
                    .NotEmpty().WithMessage("Answer cannot be empty");

                answer.RuleFor(a => a.TimeSpentSeconds)
                    .GreaterThanOrEqualTo(0).WithMessage("Time spent cannot be negative");
            });
    }
} 