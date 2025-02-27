using System.Text.Json.Serialization;

namespace yyLib
{
    // Supports assistant messages' content's text/image/audio content parts and refusal content parts.
    // I'd prefer not to make an IEnumerable of objects and determine the type of each object.

    public class yyGptChatContentPart
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }

        [JsonPropertyName ("text")]
        public string? Text { get; set; }

        [JsonPropertyName ("image_url")]
        public yyGptChatImageUrl? ImageUrl { get; set; }

        [JsonPropertyName ("input_audio")]
        public yyGptChatAudio? InputAudio { get; set; }

        [JsonPropertyName ("refusal")]
        public string? Refusal { get; set; }
    }
}
