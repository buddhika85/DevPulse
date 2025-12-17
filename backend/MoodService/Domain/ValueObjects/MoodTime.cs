using SharedLib.Domain.ValueObjects;

namespace MoodService.Domain.ValueObjects
{
    public sealed class MoodTime : ValueObject
    {
        public string Value { get; }
        public string TimeRange { get; }

        private MoodTime(string value, string timeRange)
        {
            Value = value;
            TimeRange = timeRange;
        }

        // 8 AM to 10:59 AM
        public static readonly MoodTime MorningSession =
            new("Morning", "8:00 AM - 10:59 AM");

        // 11 AM to 2:59 PM
        public static readonly MoodTime MidDaySession =
            new("MidDay", "11:00 AM - 2:59 PM");

        // 3 PM to 6 PM
        public static readonly MoodTime EveningSession =
            new("Evening", "3:00 PM - 6:00 PM");

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return TimeRange;
        }

        public static MoodTime From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("MoodTime cannot be null or empty.", nameof(value));

            return value.Trim().ToLowerInvariant() switch
            {
                "morningsession" => MorningSession,
                "middaysession" => MidDaySession,
                "eveningsession" => EveningSession,

                _ => throw new ArgumentException($"Invalid MoodTime: {value}")
            };
        }

        public override string ToString() => Value;
    }
}