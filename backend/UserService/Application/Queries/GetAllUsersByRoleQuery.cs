using MediatR;
using UserService.Application.Dtos;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Queries
{
    // GetAllUsersByRoleQueryValidator
    public record GetAllUsersByRoleQuery(UserRole Role) : IRequest<IReadOnlyList<UserAccountDto>> { }
}
