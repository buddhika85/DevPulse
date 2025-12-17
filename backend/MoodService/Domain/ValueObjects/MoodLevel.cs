using SharedLib.Domain.ValueObjects;

namespace MoodService.Domain.ValueObjects
{
    public sealed class MoodLevel : ValueObject
    {
        public string Value { get; }
        public int Score { get; }

        private MoodLevel(string value, int score)
        {
            Value = value;
            Score = score;
        }

        // Positive (+2)
        public static readonly MoodLevel Happy = new("Happy", 2);
        public static readonly MoodLevel Grateful = new("Grateful", 2);

        // Positive (+1)
        public static readonly MoodLevel Calm = new("Calm", 1);
        public static readonly MoodLevel Motivated = new("Motivated", 1);

        // Neutral (0)
        public static readonly MoodLevel Neutral = new("Neutral", 0);

        // Negative (–1)
        public static readonly MoodLevel Tired = new("Tired", -1);
        public static readonly MoodLevel Stressed = new("Stressed", -1);
        public static readonly MoodLevel Frustrated = new("Frustrated", -1);

        // Negative (–2)
        public static readonly MoodLevel Sad = new("Sad", -2);
        public static readonly MoodLevel Overwhelmed = new("Overwhelmed", -2);

        // Factory Method
        public static MoodLevel From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("MoodLevel cannot be null or empty.", nameof(value));

            return value.Trim().ToLowerInvariant() switch
            {
                "happy" => Happy,
                "grateful" => Grateful,

                "calm" => Calm,
                "motivated" => Motivated,

                "neutral" => Neutral,

                "tired" => Tired,
                "stressed" => Stressed,
                "frustrated" => Frustrated,

                "sad" => Sad,
                "overwhelmed" => Overwhelmed,

                _ => throw new ArgumentException($"Invalid MoodLevel: {value}")
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Score;
        }

        public override string ToString() => Value;
    }
}
