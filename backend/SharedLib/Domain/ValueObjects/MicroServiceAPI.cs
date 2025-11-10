namespace SharedLib.Domain.ValueObjects
{
    public class MicroServiceAPI : ValueObject
    {
        public string Value { get; }

        private MicroServiceAPI(string value) => Value = value;

        // Static instance representing each MicroServiceAPI
        public static readonly MicroServiceAPI UserAPI = new("UserAPI");
        public static readonly MicroServiceAPI TaskAPI = new("TaskAPI");

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
