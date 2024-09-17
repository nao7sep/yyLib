using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatRequestResponseFormat
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }
    }
}
