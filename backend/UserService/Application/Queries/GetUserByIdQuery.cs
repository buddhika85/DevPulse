using MediatR;
using SharedLib.DTOs.User;

namespace UserService.Application.Queries
{   
    public record GetUserByIdQuery(Guid Id): IRequest<UserAccountDto?> 
    { }
}
