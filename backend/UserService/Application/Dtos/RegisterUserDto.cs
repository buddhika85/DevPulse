namespace UserService.Application.Dtos
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
    }
}
