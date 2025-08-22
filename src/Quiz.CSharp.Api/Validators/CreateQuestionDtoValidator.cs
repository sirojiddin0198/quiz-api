namespace Quiz.CSharp.Api.Validators;

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator(IValidator<CreateQuestionRequest> baseValidator)
    {
        Include(baseValidator);
        
        RuleFor(x => x.CollectionId)
            .GreaterThan(0).WithMessage("CollectionId must be greater than zero.");
    }
}
