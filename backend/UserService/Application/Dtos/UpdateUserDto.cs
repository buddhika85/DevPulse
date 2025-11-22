namespace UserService.Application.Dtos
{
    public record UpdateUserDto(string? Email, string DisplayName, string Role, Guid? ManagerId);
}
