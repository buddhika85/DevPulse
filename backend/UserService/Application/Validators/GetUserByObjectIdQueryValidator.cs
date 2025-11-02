using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class GetUserByObjectIdQueryValidator : AbstractValidator<GetUserByObjectIdQuery>
    {
        public GetUserByObjectIdQueryValidator()
        {
            RuleFor(x => x.ObjectId)
                .NotEmpty()
                    .WithMessage("ObjectId is required to fetch the user profile.")
                .Must(BeValidGuid)
                    .WithMessage("ObjectId must be a valid GUID.");


        }


        private bool BeValidGuid(string objectId) =>
            Guid.TryParse(objectId, out _);

    }
}
