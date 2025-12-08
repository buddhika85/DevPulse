using Newtonsoft.Json;

namespace SharedLib.Domain.ValueObjects.Converters
{
    /// <summary>
    /// Custom JSON converter for UserRole when using Newtonsoft.Json.
    /// Ensures UserRole can be serialized as a string and deserialized back via UserRole.From().
    /// </summary>
    public class UserRoleNewtonsoftConverter : JsonConverter
    {
        // Tell Newtonsoft whether this converter can handle the given type.
        public override bool CanConvert(Type objectType)
            => objectType == typeof(UserRole);

        // Called during deserialization (JSON → object).
        // Reads the string value from JSON and uses UserRole.From() to create the correct instance.
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = reader.Value?.ToString();
            if (string.IsNullOrEmpty(str))
                throw new JsonSerializationException("Role cannot be null or empty.");

            return UserRole.From(str);
        }

        // Called during serialization (object → JSON).
        // Writes the UserRole's Value property back into the JSON output as a string.
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var role = (UserRole)value;
            writer.WriteValue(role.Value);
        }
    }
}