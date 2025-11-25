namespace UserService.Configuration
{
    // Entra Id settings for getting backend API token and user profiled reads
    public class EntraExternalIdSettings
    {
        public string Instance { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string Domain { get; set; } = default!;
        public string[] Audiences { get; set; } = default!;
        public string ClientSecret { get; set; } = default!; 
    }
}
