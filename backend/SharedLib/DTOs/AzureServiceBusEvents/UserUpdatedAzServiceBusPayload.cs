namespace SharedLib.DTOs.AzureServiceBusEvents
{
    public class UserUpdatedAzServiceBusPayload
    {
        public required string UserId { get; set; }
        public string Message => "User updated";
        public DateTime TimeStamp => DateTime.UtcNow;
    }
}
