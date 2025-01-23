using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptImagesRequest
    {
        // https://platform.openai.com/docs/api-reference/images/create

        [JsonPropertyName ("prompt")]
        public required string Prompt { get; set; }

        // Surprisingly, an optional parameter.
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
