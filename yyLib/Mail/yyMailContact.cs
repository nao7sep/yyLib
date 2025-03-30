using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyMailContact
    {
        [JsonPropertyName ("name")]
        public string? Name { get; set; }

        [JsonPropertyName ("address")]
        public required string Address { get; set; }

        // Could be a list of enums, but better to keep it flexible.
        // https://botpress.com/blog/list-of-languages-supported-by-chatgpt
        // https://www.mlyearning.org/languages-supported-by-chatgpt/

        /// <summary>
        /// In order of preference.
        /// </summary>
        [JsonPropertyName ("preferred_languages")]
        public IList <string>? PreferredLanguages { get; set; }

        [JsonPropertyName ("preferred_body_format")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public yyMailBodyFormat? PreferredBodyFormat { get; set; }

        public void AddPreferredLanguage (string language) => (PreferredLanguages ??= []).Add (language);
    }
}
