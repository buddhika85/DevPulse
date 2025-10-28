using MediatR;

namespace UserService.Application.Queries
{
    // ExistsByEmailQueryValidator
    public record ExistsByEmailQuery(string Email) : IRequest<bool> { }
}
