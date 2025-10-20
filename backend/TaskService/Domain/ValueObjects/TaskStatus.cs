using SharedLib.Domain.ValueObjects;

namespace TaskService.Domain.ValueObjects
{
    // TaskStatus is a value object representing the status of a task.
    // It encapsulates the concept of status using predefined static instances.
    // Since it's a value object, equality is based on its data (Value), not identity.
    public sealed class TaskStatus : ValueObject
    {
        // The underlying string value of the status (e.g., "Pending", "Completed").
        // no setter - Immutable
        public string Value { get; }

        // Private constructor ensures controlled creation through static properties.
        // This enforces immutability and restricts status values to known options.
        private TaskStatus(string value) => Value = value;

        // Static instance representing a pending task.
        public static TaskStatus Pending => new("Pending");

        // Static instance representing a completed task.
        public static TaskStatus Completed => new("Completed");

        // Factory Method to return a TaskStatus from a string value
        public static TaskStatus From(string value)
        {
            return value switch
            {
                "Pending" => Pending,
                "Completed" => Completed,
                _ => throw new ArgumentException($"Invalid TaskStatus: {value}")
            };
        }


        // Defines which properties are used to determine equality between value objects.
        // In this case, two TaskStatus instances are equal if their Value strings match.
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}