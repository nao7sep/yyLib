using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports:
    //     assistant message/tool_call
    //     request/tool
    //     request/tool_choice
    //     response/choice/message/tool_call
    //     chunk response/choice/delta/tool_call

    public class yyGptChatTool
    {
        [JsonPropertyName ("id")]
        public string? Id { get; set; }

        [JsonPropertyName ("type")]
        public string? Type { get; set; }

        [JsonPropertyName ("function")]
        public yyGptChatFunction? Function { get; set; }

        [JsonPropertyName ("index")]
        public int? Index { get; set; }
    }
}
