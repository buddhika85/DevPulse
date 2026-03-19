namespace SharedLib.Helpers
{
    public static class TimeHelper
    {
        public static string GetAusSydTime()
        {
            var syd = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, syd)
                               .ToString("hh:mm tt");
        }
    }
}
