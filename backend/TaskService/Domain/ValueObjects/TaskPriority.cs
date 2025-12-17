using SharedLib.Domain.ValueObjects;

namespace TaskService.Domain.ValueObjects
{
    public sealed class TaskPriority : ValueObject
    {
        public string Value { get; }

        private TaskPriority(string value) => Value = value;

        public static TaskPriority High => new("High");
        public static TaskPriority Low => new("Low");
        public static TaskPriority Medium => new("Medium");

        public static TaskPriority From(string value) 
        {
            return value.ToLower() switch
            {
                "high" => High,
                "medium" => Medium,
                "low" => Low,
                _ => throw new ArgumentException($"Invalid TaskPriority: {value}")
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
