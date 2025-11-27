using SharedLib.Domain.ValueObjects;
using SharedLib.DTOs.User;
using System.Text.Json;
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
                UserRole = entity.Role.Value,
                ManagerId = entity.ManagerId,
                ManagerName = entity.Manager?.DisplayName
            };
        }

        public static IEnumerable<UserAccountDto> ToDtosList(IEnumerable<UserAccount> entities)
        {
            return entities.Select(ToDto);
        }


        // returns UserAccountDto? from Entra returned graphUser object
        public static UserAccountDto? FromGraphUser(JsonElement graphUser)
        {
            try
            {
                var upn = graphUser.GetProperty("userPrincipalName").GetString();
                var email = ExtractEmailFromUpn(upn); // see method below

                return new UserAccountDto
                {
                    Id = Guid.Parse(graphUser.GetProperty("id").GetString() ?? Guid.Empty.ToString()),
                    Email = email,
                    DisplayName = graphUser.GetProperty("displayName").GetString() ?? string.Empty,
                    UserRole = UserRole.User.Value,                                                             // default 
                    CreatedAt = DateTime.UtcNow.ToString("o")
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        // extracts email from abc_gmail.com#EXT#@abcgmail.onmicrosoft.com
        public static string ExtractEmailFromUpn(string? upn)
        {
            if (string.IsNullOrWhiteSpace(upn)) return string.Empty;

            var parts = upn.Split("#EXT#");
            if (parts.Length > 0)
            {
                return parts[0].Replace("_", "@");
            }

            return string.Empty;
        }

    }
}
