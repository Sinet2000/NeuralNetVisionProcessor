using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetVisionProc.Common.Serialization
{
    [ExcludeFromCodeCoverage]
    public class JsonStringTrimConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            string? value = reader.GetString();

            return !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(string.IsNullOrWhiteSpace(value) ? null : value.Trim());
        }
    }
}