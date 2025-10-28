using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class GetUserByIdQueryValidator: AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id must not be empty.");
        }
    }
}
