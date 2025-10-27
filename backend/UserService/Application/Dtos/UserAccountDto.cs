namespace UserService.Application.Dtos
{
    public class UserAccountDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }
}
