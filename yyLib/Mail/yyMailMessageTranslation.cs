using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyMailMessageTranslation
    {
        [JsonPropertyName ("translated_at_utc")]
        public DateTime? TranslatedAtUtc { get; set; }

        [JsonPropertyName ("language")]
        public string? Language { get; set; }

        [JsonPropertyName ("text")]
        public string? Text { get; set; }

        // I thought of renaming these to "Data" and "AddData."
        // ChatGPT explains why I dont need to:

        // The "Details" collection is designed to store key-value pairs, where each pair represents a specific attribute or property of an entity.
        // Each entry in "Details" encapsulates an individual piece of information, effectively characterizing the detailed aspects of the entity.
        // The method "AddDetail" appropriately describes its functionality: it adds a new key-value pair to the "Details" collection.
        // This naming convention is not only semantically clear but also aligns with common programming practices,
        // where such terms are used to denote comprehensive and granular data elements.
        // Using "Details" and "AddDetail" enhances both the readability and the intuitiveness of the code,
        // aiding in maintenance and understanding for anyone reviewing or extending this codebase in the future.

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
