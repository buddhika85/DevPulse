namespace SharedLib.Configuration.jwt
{
    public class DevPulseJwtSettings
    {
        public string Key { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string[] Audiences { get; set; } = default!;
        public byte ExpiryHours { get; set; } = 1;
    }
}
