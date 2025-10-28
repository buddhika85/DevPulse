using UserService.Application.Commands;
using UserService.Application.Dtos;
using UserService.Domain.Entities;

namespace TaskService.Application.Common.Mappers
{
    public static class UserAccountMapper
    {
        public static UserAccountDto ToDto(UserAccount entity)
        {
            return new UserAccountDto
            {
                Id = entity.Id,
                Email = entity.Email,
                DisplayName = entity.DisplayName,
                CreatedAt = entity.CreatedAt.ToShortDateString(),
                UserRole = entity.Role.Value
            };
        }

        public static IEnumerable<UserAccountDto> ToDtosList(IEnumerable<UserAccount> entities)
        {
            return entities.Select(ToDto);
        }
    }
}
