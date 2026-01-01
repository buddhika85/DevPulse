namespace OrchestratorService.Configurations
{
    public class MicroServicesUrlSettings
    {
        public string UserAPI { get; set; } = string.Empty;
        public string TaskAPI { get; set; } = string.Empty;

        public string MoodAPI { get; set; } = string.Empty;

        public string JounralAPI { get; set; } = string.Empty;

        public string TaskJournalLinkAPI { get; set; } = string.Empty;
    }

    public class PollyConfig
    {
        public byte RetryCount { get; set; } = 1;
        public ushort SleepDurationMilliSeconds { get; set; } = 300;
    }
}
