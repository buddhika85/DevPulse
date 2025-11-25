namespace SharedLib.DTOs.User
{
    public class UserProfileResponseDto
    {
        public required UserAccountDto User { get; set; }
        public required string DevPulseJwToken { get; set; }
    }
}
