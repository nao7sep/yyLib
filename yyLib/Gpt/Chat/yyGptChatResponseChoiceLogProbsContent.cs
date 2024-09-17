using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatResponseChoiceLogProbsContent
    {
        // Deserializes "choices/logprobs/content":
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
        // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

        // The properties are sorted in the order of the API reference.

        [JsonPropertyName ("token")]
        public string? Token { get; set; }

        [JsonPropertyName ("logprob")]
        public double? LogProb { get; set; }

        [JsonPropertyName ("bytes")]
        public IList <int>? Bytes { get; set; }

        [JsonPropertyName ("top_logprobs")]
        public IList <yyGptChatResponseChoiceLogProbsContentTopLogProbs>? TopLogProbs { get; set; }
    }
}
