namespace SharedLib.Configuration
{
    public class AzureAdB2CSettings
    {
        public string Instance { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string SignUpSignInPolicyId { get; set; } = string.Empty;
    }
}
