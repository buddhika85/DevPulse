using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class GetUsersPaginatedQueryValidator : AbstractValidator<GetUsersPaginatedQuery>
    {
        private static readonly string[] AllowedRoles = { "user", "manager", "admin" };

        public GetUsersPaginatedQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be atleast 1");

            RuleFor(x => x.PageSize)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must not exceed 100.");             // prevent abuse with unusual inputs

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(2)
                .WithMessage("Page number must be atleast 2");

            RuleFor(x => x.SortBy)
                .IsInEnum()
                .When(x => x.SortBy.HasValue)
                .WithMessage("SortBy must be a valid field.");


            RuleFor(x => x.Role)
                    // if provided it should be either - manager, Manager, User, user, admin, Admin
                .Must(role => !string.IsNullOrWhiteSpace(role) && AllowedRoles.Contains(role.ToLower()))
                .WithMessage("Role must be one of: User, Manager, Admin.")
                .When(x => !string.IsNullOrWhiteSpace(x.Role));                     // conditional validation
        }
    
    }
}
