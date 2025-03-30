using System.Diagnostics.CodeAnalysis;
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

        [JsonPropertyName ("id")]
        public string? Id { get; set; }

        [JsonPropertyName ("choices")]
        public IList <yyGptChatChoice>? Choices { get; set; }

        [JsonPropertyName ("created")]
        public int? Created { get; set; }

        [JsonPropertyName ("model")]
        public string? Model { get; set; }

        [JsonPropertyName ("service_tier")]
        public string? ServiceTier { get; set; }

        [JsonPropertyName ("system_fingerprint")]
        public string? SystemFingerprint { get; set; }

        [JsonPropertyName ("object")]
        // Disables the warning for using a member name that may conflict with a type name (CA1720).
        [SuppressMessage ("Naming", "CA1720")]
        public string? Object { get; set; }

        [JsonPropertyName ("usage")]
        public yyGptChatUsage? Usage { get; set; }

        [JsonPropertyName ("error")]
        public yyGptError? Error { get; set; }

        // -----------------------------------------------------------------------------
        // Static Members
        // -----------------------------------------------------------------------------

        /// <summary>
        /// Returned by yyGptChatResponseParser.ParseChunk when "data: [DONE]" is detected.
        /// </summary>
        public static yyGptChatResponse Empty { get; } = new ();
    }
}
