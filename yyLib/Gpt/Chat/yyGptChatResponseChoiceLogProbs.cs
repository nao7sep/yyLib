using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatResponseChoiceLogProbs
    {
        // Deserializes "choices/logprobs":
        // https://platform.openai.com/docs/api-reference/chat/object#chat/object-choices
        // https://platform.openai.com/docs/api-reference/chat/streaming#chat/streaming-choices

        [JsonPropertyName ("content")]
        public IList <yyGptChatResponseChoiceLogProbsContent>? Content { get; set; }
    }
}
