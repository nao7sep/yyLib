using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatPrediction
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }

        [JsonPropertyName ("content")]
        public object? Content { get; set; }
    }
}
