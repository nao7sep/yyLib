using System.Text.Json.Serialization;

namespace yyLib
{
    // Because "request/tools/function" contains significantly different properties,
    // this model class supports "function_call" and also "tool_calls/function"
    // while yyGptChatFunction supports "request/tools/function".

    public class yyGptChatFunctionCall
    {
        [JsonPropertyName ("name")]
        public string? Name { get; set; }

        [JsonPropertyName ("arguments")]
        public string? Arguments { get; set; }
    }
}
