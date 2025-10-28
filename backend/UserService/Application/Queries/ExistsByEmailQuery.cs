using MediatR;

namespace UserService.Application.Queries
{
    public record ExistsByEmailQuery(string Email) : IRequest<bool> { }
}
