using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports:
    //     assistant message/tool_call/function
    //     assistant message/function_call
    //     request/tool/function
    //     request/function_call
    //     chunk response/choice/delta/function_call

    public class yyGptChatFunction
    {
        [JsonPropertyName ("name")]
        public string? Name { get; set; }

        [JsonPropertyName ("arguments")]
        public string? Arguments { get; set; }

        [JsonPropertyName ("description")]
        public string? Description { get; set; }

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
