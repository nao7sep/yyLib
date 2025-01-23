using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace yyLib
{
    // Used as a list for the Data property.
    // Considered "image", "entry", "datum", etc.
    public class yyGptImagesImage
    {
        // https://platform.openai.com/docs/api-reference/images/object

        [JsonPropertyName ("b64_json")]
        public string? B64Json { get; set; }

        [JsonPropertyName ("url")]
        // Suppresses the warning about using string properties to represent URIs instead of Uri objects (CA1056).
        [SuppressMessage ("Design", "CA1056")]
        public string? Url { get; set; }

        [JsonPropertyName ("revised_prompt")]
        public string? RevisedPrompt { get; set; }
    }
}
