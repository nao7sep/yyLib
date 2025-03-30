﻿using System.Text.Json.Serialization;

namespace yyLib
{
    // Used for:
    //     user message/audio content part
    //     assistant message/audio
    //     request/audio
    //     response/choice/message/audio

    public class yyGptChatAudio
    {
        /// <summary>
        /// Base64 encoded audio bytes generated by the model, in the format specified in the request.
        /// </summary>
        [JsonPropertyName ("data")]
        public string? Data { get; set; }

        /// <summary>
        /// Must be one of wav, mp3, flac, opus, or pcm16.
        /// </summary>
        [JsonPropertyName ("format")]
        public string? Format { get; set; }

        /// <summary>
        /// Unique identifier for this audio response.
        /// </summary>
        [JsonPropertyName ("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Supported voices are ash, ballad, coral, sage, and verse (also supported but not recommended are alloy, echo, and shimmer; these voices are less expressive).
        /// </summary>
        [JsonPropertyName ("voice")]
        public string? Voice { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when this audio response will no longer be accessible on the server for use in multi-turn conversations.
        /// </summary>
        [JsonPropertyName ("expires_at")]
        public int? ExpiresAt { get; set; }

        /// <summary>
        /// Transcript of the audio generated by the model.
        /// </summary>
        [JsonPropertyName ("transcript")]
        public string? Transcript { get; set; }
    }
}
