using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatToolCall
    {
        // Needed for streaming.
        [JsonPropertyName ("index")]
        public int? Index { get; set; }

        [JsonPropertyName ("id")]
        public string? Id { get; set; }

        [JsonPropertyName ("type")]
        public string? Type { get; set; }

        [JsonPropertyName ("function")]
        public yyGptChatFunction? Function { get; set; }
    }
}
