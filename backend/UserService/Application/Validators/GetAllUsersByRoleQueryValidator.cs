using FluentValidation;
using UserService.Application.Queries;

namespace UserService.Application.Validators
{
    public class GetAllUsersByRoleQueryValidator : AbstractValidator<GetAllUsersByRoleQuery> 
    {
        private static readonly string[] AllowedRoles = { "user", "manager", "admin" };
        public GetAllUsersByRoleQueryValidator()
        {
            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(role => AllowedRoles.Contains(role.Value.ToLower()))
                 .WithMessage("Role must be one of: User, Manager, Admin.");
        }
    }
}
