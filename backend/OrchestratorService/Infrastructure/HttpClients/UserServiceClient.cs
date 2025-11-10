using SharedLib.DTOs.User;


namespace OrchestratorService.Infrastructure.HttpClients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserAccountDto> GetUserAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var userIdGuid))
                throw new ArgumentException("Invalid user ID format");

            // call user API to get user by Id
            var user = await _httpClient.GetFromJsonAsync<UserAccountDto>($"api/profile/{userIdGuid}");

            return user  
                ?? throw new InvalidOperationException("User not found");
        }
    }

}
