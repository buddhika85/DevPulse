namespace OrchestratorService.Configurations
{
    public class MicroServicesUrlSettings
    {
        public string UserAPI { get; set; } = string.Empty;
        public string TaskAPI { get; set; } = string.Empty;
    }

    public class PollyConfig
    {
        public byte RetryCount { get; set; } = 1;
        public ushort SleepDurationMilliSeconds { get; set; } = 300;
    }
}
