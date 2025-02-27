using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports:
    //     response/choice/logprobs/content/top_logprobs
    //     response/choice/logprobs/refusal/top_logprobs
    //     chunk response/choice/logprobs/content/top_logprobs
    //     chunk response/choice/logprobs/refusal/top_logprobs

    // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
    // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

    // Probably, yyGptChatLogProbsContent and yyGptChatTopLogProb should not be merged
    // because that could cause circular references.

    // Singular because it's used as a list named TopLogProbs.
    public class yyGptChatTopLogProb
    {
        [JsonPropertyName ("token")]
        public string? Token { get; set; }

        [JsonPropertyName ("logprob")]
        public double? LogProb { get; set; }

        [JsonPropertyName ("bytes")]
        public IList <int>? Bytes { get; set; }
    }
}
