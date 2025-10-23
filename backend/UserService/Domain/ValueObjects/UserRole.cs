using SharedLib.Domain.ValueObjects;

namespace UserService.Domain.ValueObjects
{
    public class UserRole : ValueObject
    {
        // The underlying string value of the role (e.g., "Admin", "Manager", "User"....).
        // no setter - Immutable
        public string Value { get; }

        // Private constructor ensures controlled creation through static properties.
        // This enforces immutability and restricts status values to known options.
        private UserRole(string value) => Value = value;

        // Static instance representing each role
        public static readonly UserRole User = new("User");
        public static readonly UserRole Manager = new("Manager");
        public static readonly UserRole Admin = new("Admin");

        // Factory Method to return a UserRole from a string value
        public static UserRole From(string value) =>
            value.ToLower() switch
            {
                "user" => User,
                "manager" => Manager,
                "admin" => Admin,
                _ => throw new ArgumentException($"Invalid role: {value}")
            };

        // Defines which properties are used to determine equality between value objects.
        // In this case, two roles instances are equal if their Value strings match.
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
