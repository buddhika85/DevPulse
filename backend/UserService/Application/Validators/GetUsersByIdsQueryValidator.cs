using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class GetUsersByIdsQueryValidator : AbstractValidator<GetUsersByIdsQuery>
    {
        public GetUsersByIdsQueryValidator()
        {
            RuleFor(x => x.UserIds)
                .NotEmpty().WithMessage("User Ids must not be empty.");
        }
    }
}
