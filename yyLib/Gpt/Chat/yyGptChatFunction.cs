using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports "request/tools/function".

    public class yyGptChatFunction
    {
        [JsonPropertyName ("description")]
        public string? Description { get; set; }

        [JsonPropertyName ("name")]
        public string? Name { get; set; }

        // Official documentation says: The parameters the functions accepts, described as a JSON Schema object.
        // See the guide (https://platform.openai.com/docs/guides/function-calling) for examples,
        // and the JSON Schema reference (https://json-schema.org/understanding-json-schema) for documentation about the format.
        /// <summary>
        /// Must be object or null.
        /// </summary>
        [JsonPropertyName ("parameters")]
        public object? Parameters { get; set; }

        [JsonPropertyName ("strict")]
        public bool? Strict { get; set; }
    }
}
