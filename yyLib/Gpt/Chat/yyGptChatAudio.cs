using System.Text.Json.Serialization;

namespace yyLib
{
    // Used for:
    //     user message/audio content part
    //     assistant message/audio
    //     request/audio
    //     response/choice/message/audio

    public class yyGptChatAudio
    {
        [JsonPropertyName ("data")]
        public string? Data { get; set; }

        [JsonPropertyName ("format")]
        public string? Format { get; set; }

        [JsonPropertyName ("id")]
        public string? Id { get; set; }

        [JsonPropertyName ("voice")]
        public string? Voice { get; set; }

        [JsonPropertyName ("expires_at")]
        public int? ExpiresAt { get; set; }

        [JsonPropertyName ("transcript")]
        public string? Transcript { get; set; }
    }
}
