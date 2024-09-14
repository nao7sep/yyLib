using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptImagesRequest
    {
        // https://platform.openai.com/docs/api-reference/images/create

        // The properties are sorted in the order of the API reference.

        [JsonPropertyName ("prompt")]
        public string? Prompt { get; set; }

        // There's no default value for this property because it's optional.
        // Chat completions' Model, on the other hand, is required and has a default value.

        [JsonPropertyName ("model")]
        public string? Model { get; set; }

        [JsonPropertyName ("n")]
        public int? N { get; set; }

        [JsonPropertyName ("quality")]
        public string? Quality { get; set; }

        [JsonPropertyName ("response_format")]
        public string? ResponseFormat { get; set; }

        [JsonPropertyName ("size")]
        public string? Size { get; set; }

        [JsonPropertyName ("style")]
        public string? Style { get; set; }

        [JsonPropertyName ("user")]
        public string? User { get; set; }
    }
}
