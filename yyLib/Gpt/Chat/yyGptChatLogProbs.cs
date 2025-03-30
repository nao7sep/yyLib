using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports:
    //     response/choice/logprobs
    //     chunk response/choice/logprobs

    // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
    // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

    // Plural because it contains a list and represents the "logprobs" object.
    public class yyGptChatLogProbs
    {
        [JsonPropertyName ("content")]
        public IList <yyGptChatLogProbsContent>? Content { get; set; }

        [JsonPropertyName ("refusal")]
        public IList <yyGptChatLogProbsContent>? Refusal { get; set; }
    }
}
