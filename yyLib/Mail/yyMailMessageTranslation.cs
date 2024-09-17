using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyMailMessageTranslation
    {
        /// <summary>
        /// When the translation was made. Not the time of object initialization.
        /// </summary>
        [JsonPropertyName ("utc")]
        public DateTime? Utc { get; set; }

        [JsonPropertyName ("language")]
        public string? Language { get; set; }

        [JsonPropertyName ("text")]
        public string? Text { get; set; }

        /// <summary>
        /// Should contain name of translator, API endpoint, parameters, etc. Just enough info to (try to) replicate the process.
        /// </summary>
        [JsonPropertyName ("details")]
        public IDictionary <string, string>? Details { get; set; }

        public void AddDetail (string key, string value)
        {
            Details ??= new Dictionary <string, string> ();
            Details.Add (key, value);
        }
    }
}
