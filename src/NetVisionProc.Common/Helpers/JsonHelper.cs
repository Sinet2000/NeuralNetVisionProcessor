using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace NetVisionProc.Common.Helpers
{
    /// <summary>
    /// Json serializer helper to support Unicode and Enums as strings.
    /// Author: Nikita Nikitins .
    /// Email: nikitinsn6@gmail.com .
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class JsonHelper
    {
        // Lazy initialization of JSON options for indented serialization
        private static Lazy<JsonSerializerOptions> JsonOptIndentedLazy => new(GetJsonOptionsIndented);

        /// <summary>
        /// Serializes an object to an indented JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>An indented JSON string representing the serialized object.</returns>
        public static string SerializeIndented<T>(T value)
        {
            return JsonSerializer.Serialize(value, JsonOptIndentedLazy.Value);
        }

        // Configures JSON options for indented serialization
        private static JsonSerializerOptions GetJsonOptionsIndented()
        {
            var jsonOpt = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            jsonOpt.Converters.Add(new JsonStringEnumConverter());

            return jsonOpt;
        }
    }
}