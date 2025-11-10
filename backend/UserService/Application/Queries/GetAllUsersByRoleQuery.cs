using MediatR;
using SharedLib.DTOs.User;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Queries
{
    // GetAllUsersByRoleQueryValidator
    public record GetAllUsersByRoleQuery(UserRole Role) : IRequest<IReadOnlyList<UserAccountDto>> { }
}
