using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatMessage
    {
        // Deserializes the "messages" property in "choices":
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices

        // This model class is used both in requests and responses.
        // That's why the name is more generic.

        // Function-related things are not currently supported.

        // The properties are sorted in the natural order except for the "name" property that is optional.

        // Having discovered that the default binder for IConfiguration ignores JSON-related attributes including JsonConverter,
        // we could remove the converter here, hoping OpenAI's API will accept message roles case-insensitively,
        // OR add a property like "RoleString" for serialization/deserialization.
        // But a good thing is that the model classes and their serialization/deserialization mechanisms are packed together.
        // As long as the JSON output is correct, we can change how values are converted without harming existing code that relies on them.

        [JsonPropertyName ("role")]
        [JsonConverter (typeof (yyGptChatRoleJsonConverter))]
        public yyGptChatRole? Role { get; set; }

        [JsonPropertyName ("content")]
        public string? Content { get; set; }

        [JsonPropertyName ("name")]
        public string? Name { get; set; }
    }
}
