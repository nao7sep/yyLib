﻿using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatRequest
    {
        // Everything except function-related things.
        // https://platform.openai.com/docs/api-reference/chat/create
        // https://github.com/openai/openai-dotnet/blob/main/src/Custom/Chat/ChatCompletionOptions.cs

        // The properties are sorted in the order of the API reference.

        [JsonPropertyName ("messages")]
        public IList <yyGptChatMessage>? Messages { get; set; }

        [JsonPropertyName ("model")]
        public string? Model { get; set; }

        [JsonPropertyName ("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }

        [JsonPropertyName ("logit_bias")]
        public IDictionary <int, double>? LogitBias { get; set; }

        [JsonPropertyName ("logprobs")]
        public bool? LogProbs { get; set; }

        [JsonPropertyName ("top_logprobs")]
        public int? TopLogProbs { get; set; }

        [JsonPropertyName ("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonPropertyName ("n")]
        public int? N { get; set; }

        [JsonPropertyName ("presence_penalty")]
        public double? PresencePenalty { get; set; }

        [JsonPropertyName ("response_format")]
        public yyGptChatRequestResponseFormat? ResponseFormat { get; set; }

        [JsonPropertyName ("seed")]
        public int? Seed { get; set; }

        [JsonPropertyName ("stop")]
        public IList <string>? Stop { get; set; }

        [JsonPropertyName ("stream")]
        public bool? Stream { get; set; }

        [JsonPropertyName ("temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName ("top_p")]
        public double? TopP { get; set; }

        [JsonPropertyName ("user")]
        public string? User { get; set; }

        public yyGptChatRequest () => Model = yyGptChat.DefaultModel;
    }
}
