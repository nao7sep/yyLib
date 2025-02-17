﻿using System.Text.Json.Serialization;

namespace yyLib
{
    // Singular because it's used as a list named TopLogProbs.
    public class yyGptChatTopLogProb
    {
        // Deserializes "choices/logprobs/content/top_logprobs":
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
        // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

        [JsonPropertyName ("token")]
        public string? Token { get; set; }

        [JsonPropertyName ("logprob")]
        public double? LogProb { get; set; }

        [JsonPropertyName ("bytes")]
        public IList <int>? Bytes { get; set; }
    }
}
