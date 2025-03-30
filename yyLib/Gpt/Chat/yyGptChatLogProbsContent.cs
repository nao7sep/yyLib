using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports:
    //     response/choice/logprobs/content
    //     response/choice/logprobs/refusal
    //     chunk response/choice/logprobs/content
    //     chunk response/choice/logprobs/refusal

    // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
    // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

    // Probably, yyGptChatLogProbsContent and yyGptChatTopLogProb should not be merged
    // because that could cause circular references.

    public class yyGptChatLogProbsContent
    {
        [JsonPropertyName ("token")]
        public string? Token { get; set; }

        [JsonPropertyName ("logprob")]
        public double? LogProb { get; set; }

        [JsonPropertyName ("bytes")]
        public IList <int>? Bytes { get; set; }

        [JsonPropertyName ("top_logprobs")]
        public IList <yyGptChatTopLogProb>? TopLogProbs { get; set; }
    }
}
