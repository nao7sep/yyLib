using System.Text.Json.Serialization;

namespace yyLib
{
    // Contains "Content" that works just like the one in yyGptChatMessage.
    // But I've decided to prepare 2 classes because "Type" makes this model more like yyGptChatContentPart.
    // It's somewhere in between and not significantly closer to either of them.
    // Also, it just feels wrong to use a message model for "prediction" because it's not a message.

    public class yyGptChatPrediction
    {
        [JsonPropertyName ("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Must be string or List <yyGptChatContentPart> or null.
        /// </summary>
        [JsonPropertyName ("content")]
        [JsonConverter (typeof (yyGptChatContentJsonConverter))]
        public object? Content { get; set; }
    }
}
