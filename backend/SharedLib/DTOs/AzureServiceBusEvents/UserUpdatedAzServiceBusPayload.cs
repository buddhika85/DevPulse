namespace SharedLib.DTOs.AzureServiceBusEvents
{
    public record UserUpdatedAzServiceBusPayload(string UserId)
    {       
        public string Message { get; init; } = "User updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }

    public record UserDisplayNameChangedAzServiceBusPayload(string UserId, string OldDisplayName, string NewDisplayName, string Email)
    {        
        public string Message { get; init; } = "User Display name updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }

    public record UserEmailChangedAzServiceBusPayload(string UserId, string PreviousEmail, string NewEmail, string Email)
    {
        public string Message { get; init; } = "User Email updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }

    public record UserRoleChangedAzServiceBusPayload(string UserId, string PreviousRole, string NewRole, string Email)
    {
        public string Message { get; init; } = "User Role updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }
}
