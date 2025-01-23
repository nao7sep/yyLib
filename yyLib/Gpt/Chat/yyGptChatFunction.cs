using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports "request/tools/function".

    public class yyGptChatFunction
    {
        [JsonPropertyName ("description")]
        public string? Description { get; set; }

        [JsonPropertyName ("name")]
        public string? Name { get; set; }

        [JsonPropertyName ("parameters")]
        public object? Parameters { get; set; }

        [JsonPropertyName ("strict")]
        public bool? Strict { get; set; }
    }
}
