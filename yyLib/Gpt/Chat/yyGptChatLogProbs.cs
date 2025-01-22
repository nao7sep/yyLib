using System.Text.Json.Serialization;

namespace yyLib
{
    // Plural because it contains a list and represents the "choices/logprobs" object.
    public class yyGptChatLogProbs
    {
        // Deserializes "choices/logprobs":
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
        // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

        [JsonPropertyName ("content")]
        public IList <yyGptChatContent>? Content { get; set; }

        [JsonPropertyName ("refusal")]
        public IList <yyGptChatContent>? Refusal { get; set; }
    }
}
