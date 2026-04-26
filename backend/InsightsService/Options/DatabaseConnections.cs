namespace InsightsService.Options;


public class DatabaseConnections
{
    public string UsersDb { get; set; } = string.Empty;
    public string TasksDb { get; set; } = string.Empty;
    public string MoodDb { get; set; } = string.Empty;
    public TaskJournalLinkDb TaskJournalLinkDb { get; set; } = null!;
}


public class TaskJournalLinkDb
{
    public string AccountEndpoint { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}

