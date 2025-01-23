using System.Text.Json.Serialization;

namespace yyLib
{
    // Used for "response/choices/message/audio".

    public class yyGptChatAudio
    {
        [JsonPropertyName ("id")]
        public string? Id { get; set; }

        [JsonPropertyName ("expires_at")]
        public int? ExpiresAt { get; set; }

        [JsonPropertyName ("data")]
        public string? Data { get; set; }

        [JsonPropertyName ("transcript")]
        public string? Transcript { get; set; }
    }
}
