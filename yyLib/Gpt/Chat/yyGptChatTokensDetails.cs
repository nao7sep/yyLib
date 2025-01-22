using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatTokensDetails
    {
        // Supports "completion_tokens_details" and "prompt_tokens_details"

        [JsonPropertyName ("accepted_prediction_tokens")]
        public int? AcceptedPredictionTokens { get; set; }

        [JsonPropertyName ("audio_tokens")]
        public int? AudioTokens { get; set; }

        [JsonPropertyName ("reasoning_tokens")]
        public int? ReasoningTokens { get; set; }

        [JsonPropertyName ("rejected_prediction_tokens")]
        public int? RejectedPredictionTokens { get; set; }

        [JsonPropertyName ("cached_tokens")]
        public int? CachedTokens { get; set; }
    }
}
