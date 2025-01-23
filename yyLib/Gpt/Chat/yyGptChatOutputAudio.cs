using System.Text.Json.Serialization;

namespace yyLib
{
    // Both yyGptChatInputAudio and yyGptChatOutputAudio contain Format.
    // Naturally, it would be an option to merge them to reduce redundancy.
    // But the property that uses the one for input is explicitly called "input_audio"
    // while the documentation for the one for output says: Parameters for audio output.
    // The name "yyGptChatAudio" is already used for a very different model.
    // If one has to contain the word "output", it should make sense to make the other one contain "input".
    // The final question is whether it's "output audio" or "audio output".
    // My naming convention for GPT-related model classes is that adjectives come before nouns.

    public class yyGptChatOutputAudio
    {
        [JsonPropertyName ("voice")]
        public string? Voice { get; set; }

        [JsonPropertyName ("format")]
        public string? Format { get; set; }
    }
}
