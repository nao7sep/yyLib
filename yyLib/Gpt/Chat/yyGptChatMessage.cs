using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatMessage
    {
        // Deserializes the "messages" property in "choices":
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices

        // This model class is used both in requests and responses.
        // That's why the name is more generic.

        /// <summary>
        /// Must be string or IList <yyGptChatContentPart> or null when in request.
        /// Must be string or null when in response.
        /// </summary>
        [JsonPropertyName ("content")]
        [JsonConverter (typeof (yyGptChatContentJsonConverter))]
        public object? Content { get; set; }

        // Having discovered that the default binder for IConfiguration ignores JSON-related attributes including JsonConverter,
        // we could remove the converter here, hoping OpenAI's API will accept message roles case-insensitively,
        // OR add a property like "RoleString" for serialization/deserialization.
        // But a good thing is that the model classes and their serialization/deserialization mechanisms are packed together.
        // As long as the JSON output is correct, we can change how values are converted without harming existing code that relies on them.

        [JsonPropertyName ("role")]
        [JsonConverter (typeof (yyGptChatRoleJsonConverter))]
        public yyGptChatRole? Role { get; set; }

        [JsonPropertyName ("name")]
        public string? Name { get; set; }

        [JsonPropertyName ("refusal")]
        public string? Refusal { get; set; }

        [JsonPropertyName ("audio")]
        public yyGptChatAudio? Audio { get; set; }

        [JsonPropertyName ("tool_calls")]
        public IList <yyGptChatToolCall>? ToolCalls { get; set; }

        [JsonPropertyName ("function_call")]
        public yyGptChatFunction? FunctionCall { get; set; }

        [JsonPropertyName ("tool_call_id")]
        public string? ToolCallId { get; set; }
    }
}
