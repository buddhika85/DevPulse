using MediatR;
using SharedLib.Domain.ValueObjects;
using SharedLib.DTOs.User;

namespace UserService.Application.Queries
{
    // GetAllUsersByRoleQueryValidator
    public record GetAllUsersByRoleQuery(UserRole Role) : IRequest<IReadOnlyList<UserAccountDto>> { }
}
