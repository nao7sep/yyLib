using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatResponse
    {
        // This model class can deserialize the following 2 kinds of responses:
        // https://platform.openai.com/docs/api-reference/chat/object
        // https://platform.openai.com/docs/api-reference/chat/streaming

        // It also deserializes error information provided as a JSON string representation with the only key "error".
        // We could pre-parse a portion of the response to determine if it's a valid response or an error,
        // but the developer shouldnt have to deal with the 2 model classes that can be returned in a strongly-typed language.

        // The properties are sorted in the order of the API reference.

        [JsonPropertyName ("id")]
        public string? Id { get; set; }

        [JsonPropertyName ("choices")]
        public IList <yyGptChatResponseChoice>? Choices { get; set; }

        [JsonPropertyName ("created")]
        public int? Created { get; set; }

        [JsonPropertyName ("model")]
        public string? Model { get; set; }

        [JsonPropertyName ("system_fingerprint")]
        public string? SystemFingerprint { get; set; }

        [JsonPropertyName ("object")]
        public string? Object { get; set; }

        [JsonPropertyName ("usage")]
        public yyGptChatResponseUsage? Usage { get; set; }

        [JsonPropertyName ("error")]
        public yyGptError? Error { get; set; }

        // -----------------------------------------------------------------------------
        // Static Members
        // -----------------------------------------------------------------------------

        /// <summary>
        /// Returned by yyGptChatResponseParser.ParseChunk when "data: [DONE]" is detected.
        /// </summary>
        public static yyGptChatResponse Empty { get; } = new yyGptChatResponse ();
    }
}
