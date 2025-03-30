using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatJsonSchema
    {
        // https://platform.openai.com/docs/guides/structured-outputs

        [JsonPropertyName ("description")]
        public string? Description { get; set; }

        [JsonPropertyName ("name")]
        public string? Name { get; set; }

        // Official documentation says: The schema for the response format, described as a JSON Schema object.
        /// <summary>
        /// Must be object or null.
        /// </summary>
        [JsonPropertyName ("schema")]
        public object? Schema { get; set; }

        [JsonPropertyName ("strict")]
        public bool? Strict { get; set; }
    }
}
