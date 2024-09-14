﻿using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptChatResponseUsage
    {
        // Deserializes the "usage" property:
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-usage

        // The properties are sorted in the order of the API reference.

        [JsonPropertyName ("completion_tokens")]
        public int? CompletionTokens { get; set; }

        [JsonPropertyName ("prompt_tokens")]
        public int? PromptTokens { get; set; }

        [JsonPropertyName ("total_tokens")]
        public int? TotalTokens { get; set; }
    }
}
