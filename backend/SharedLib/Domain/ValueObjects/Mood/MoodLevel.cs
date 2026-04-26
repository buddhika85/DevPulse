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

        /// <summary>
        /// Create a <see cref="MoodLevel"/> from an integer score.
        /// Valid scores: -2, -1, 0, 1, 2. When multiple moods share the same score
        /// the first defined variant is returned (for example, <see cref="Happy"/> for 2).
        /// </summary>
        /// <param name="score">Score value representing the mood.</param>
        /// <returns>Corresponding <see cref="MoodLevel"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the score is outside the supported range.</exception>
        public static MoodLevel From(int score)
        {
            return score switch
            {
                2 => Happy,
                1 => Calm,
                0 => Neutral,
                -1 => Tired,
                -2 => Sad,
                _ => throw new ArgumentException($"Invalid MoodLevel score: {score}", nameof(score))
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
