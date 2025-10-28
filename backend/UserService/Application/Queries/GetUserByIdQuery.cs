using MediatR;
using UserService.Application.Dtos;

namespace UserService.Application.Queries
{   
    public record GetUserByIdQuery(Guid Id): IRequest<UserAccountDto> 
    { }
}
