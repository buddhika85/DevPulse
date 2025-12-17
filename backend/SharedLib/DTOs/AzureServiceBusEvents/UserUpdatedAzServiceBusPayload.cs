namespace SharedLib.DTOs.AzureServiceBusEvents
{
    public abstract record BaseUserUpdatedPayload(string UserId, string UpdateField);
    public record UserUpdatedAzServiceBusPayload(string UserId) : BaseUserUpdatedPayload(UserId, "All")
    {
        public string Message { get; init; } = "User updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }

    public record UserDisplayNameChangedAzServiceBusPayload(string UserId, string OldDisplayName, string NewDisplayName, string Email) : BaseUserUpdatedPayload(UserId, "DisplayName")
    {
        public string Message { get; init; } = "User Display name updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }

    public record UserEmailChangedAzServiceBusPayload(string UserId, string PreviousEmail, string NewEmail, string Email) : BaseUserUpdatedPayload(UserId, "Email")
    {
        public string Message { get; init; } = "User Email updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }

    public record UserRoleChangedAzServiceBusPayload(string UserId, string PreviousRole, string NewRole, string Email) : BaseUserUpdatedPayload(UserId, "Role")
    {
        public string Message { get; init; } = "User Role updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }


    public record UserManagerChangedAzServiceBusPayload(string UserId, string? PreviousManagerId, string? NewManagerId, string Email) : BaseUserUpdatedPayload(UserId, "ManagerId")
    {
        public string Message { get; init; } = "User Manager updated";
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }
}
