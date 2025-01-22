using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatInputAudio
    {
        [JsonPropertyName ("data")]
        public string? Data { get; set; }

        [JsonPropertyName ("format")]
        public string? Format { get; set; }
    }
}
