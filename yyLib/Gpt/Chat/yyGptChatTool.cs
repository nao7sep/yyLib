using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatTool
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }

        [JsonPropertyName ("function")]
        public yyGptChatFunction? Function { get; set; }
    }
}
