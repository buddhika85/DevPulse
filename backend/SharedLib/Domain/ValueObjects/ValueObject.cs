namespace SharedLib.Domain.ValueObjects
{


    // Abstract base class for all Value Objects in the domain.
    // Enforces structural equality and immutability.
    public abstract class ValueObject
    {
        // Each derived ValueObject must define which properties determine equality.
        // For example, TaskStatus might return [Value] as its equality component.
        protected abstract IEnumerable<object> GetEqualityComponents();

        // Overrides default equality comparison from the base Object class.
        // By default, Object.Equals returns true only if two references point to the same memory location.
        // This override ensures that two value objects with the same data are considered equal — even if they’re different instances.
        public override bool Equals(object? obj)
        {
            // If the other object is null or not the same type, they're not equal.
            if (obj is null || obj.GetType() != GetType())
                return false;

            // Cast to ValueObject and compare equality components.
            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        // Generates a hash code based on the equality components.
        // This is important for using value objects in dictionaries, sets, etc.
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(1, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
        }

        // Enables intuitive equality comparison using == operator.
        // Calls Equals override rather than original == which compares memoty addresses
        public static bool operator ==(ValueObject left, ValueObject right)
        {
            return Equals(left, right);
        }

        // Enables intuitive inequality comparison using != operator.
        // Calls Equals override rather than original == which compares memoty addresses
        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !Equals(left, right);
        }
    }
}
