using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptChatRequestResponseFormat
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }
    }
}
