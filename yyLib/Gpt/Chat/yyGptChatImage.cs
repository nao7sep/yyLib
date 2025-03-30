using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatImage
    {
        [JsonPropertyName ("url")]
        public string? Url { get; set; }

        [JsonPropertyName ("detail")]
        public string? Detail { get; set; }
    }
}
