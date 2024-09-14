using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptChatMessage
    {
        // Deserializes the "messages" property in "choices":
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices

        // This model class is used both in requests and responses.
        // That's why the name is more generic.

        // Function-related things are not currently supported.

        // The properties are sorted in the natural order except for the "name" property that is optional.

        [JsonPropertyName ("role")]
        [JsonConverter (typeof (yyGptChatMessageRoleJsonConverter))]
        public yyGptChatMessageRole? Role { get; set; }

        [JsonPropertyName ("content")]
        public string? Content { get; set; }

        [JsonPropertyName ("name")]
        public string? Name { get; set; }
    }
}
