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

        [JsonPropertyName ("schema")]
        public object? Schema { get; set; }

        [JsonPropertyName ("strict")]
        public bool? Strict { get; set; }
    }
}
