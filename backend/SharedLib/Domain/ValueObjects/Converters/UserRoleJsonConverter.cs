using SharedLib.Domain.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLib.Domain.ValueObjects.Converters
{
    // Custom JSON converter for the UserRole value object.
    // Ensures UserRole can be correctly serialized (to string) and deserialized (from string)
    // when used in API requests/responses.

    public class UserRoleJsonConverter : JsonConverter<UserRole>
    {
        // Called during deserialization (JSON → object).
        // Reads the string from JSON and uses the UserRole.From factory method
        // to create the correct UserRole instance (e.g., "User", "Manager", "Admin").
        public override UserRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(reader.GetString()))
                throw new JsonException("Role value cannot be null or empty.");
            return UserRole.From(reader.GetString()!);
        }

        // Called during serialization (object → JSON).
        // Writes the UserRole's Value property (string) back into the JSON output.
        public override void Write(Utf8JsonWriter writer, UserRole value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Value);
    }
}
