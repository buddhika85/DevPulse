using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class ExistsByEmailQueryValidator : AbstractValidator<ExistsByEmailQuery>
    {
        public ExistsByEmailQueryValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .Matches(@"^[\w\.-]+@([\w-]+\.)+(com|net|org|edu)$")                            // regex
                .WithMessage("Email must be a valid domain (e.g., .com, .net, .org)");
        }
    }
}
