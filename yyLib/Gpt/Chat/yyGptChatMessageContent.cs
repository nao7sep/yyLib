using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatMessageContent
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }

        [JsonPropertyName ("text")]
        public string? Text { get; set; }

        [JsonPropertyName ("image_url")]
        public yyGptChatImageUrl? ImageUrl { get; set; }

        [JsonPropertyName ("input_audio")]
        public yyGptChatInputAudio? InputAudio { get; set; }
    }
}
