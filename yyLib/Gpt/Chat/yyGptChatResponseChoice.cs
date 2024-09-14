using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptChatResponseChoice
    {
        // This model class can deserialize the following 2 properties:
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
        // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

        // The properties are sorted in the order of the API reference.

        // The chunk object has a different order.
        // The order of the normal response is respected here.
        // Considering "delta" is like an equivalent of "message" in the chunk object, it's placed after "message".

        [JsonPropertyName ("finish_reason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName ("index")]
        public int? Index { get; set; }

        [JsonPropertyName ("message")]
        public yyGptChatMessage? Message { get; set; }

        [JsonPropertyName ("delta")]
        public yyGptChatMessage? Delta { get; set; }

        [JsonPropertyName ("logprobs")]
        public yyGptChatResponseChoiceLogProbs? LogProbs { get; set; }
    }
}
