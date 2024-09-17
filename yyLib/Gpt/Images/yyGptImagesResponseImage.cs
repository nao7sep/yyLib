using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptImagesResponseImage
    {
        // https://platform.openai.com/docs/api-reference/images/object

        // The properties are sorted in the order of the API reference.

        [JsonPropertyName ("b64_json")]
        public string? B64Json { get; set; }

        [JsonPropertyName ("url")]
        public string? Url { get; set; }

        [JsonPropertyName ("revised_prompt")]
        public string? RevisedPrompt { get; set; }
    }
}
