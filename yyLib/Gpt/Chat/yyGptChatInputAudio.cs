using System.Text.Json.Serialization;

namespace yyLib
{
    // Refer to the comment in yyGptChatOutputAudio.

    public class yyGptChatInputAudio
    {
        [JsonPropertyName ("data")]
        public string? Data { get; set; }

        [JsonPropertyName ("format")]
        public string? Format { get; set; }
    }
}
