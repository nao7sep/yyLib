using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatImageUrl
    {
        [JsonPropertyName ("url")]
        public string? Url { get; set; }

        [JsonPropertyName ("detail")]
        public string? Detail { get; set; }
    }
}
