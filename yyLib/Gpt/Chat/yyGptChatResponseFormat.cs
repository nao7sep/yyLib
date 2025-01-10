using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatResponseFormat
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }
    }
}
