using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyMailTranslation
    {
        [JsonPropertyName ("translated_at_utc")]
        public required DateTime TranslatedAtUtc { get; set; }

        [JsonPropertyName ("language")]
        public required string Language { get; set; }

        [JsonPropertyName ("text")]
        public required string Text { get; set; }

        /// <summary>
        /// Should contain name of translator, API endpoint, parameters, etc.
        /// Just enough info to (try to) replicate the process.
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
