using System.Text.Json.Serialization;

namespace yyLib
{
    public class yySimpleLog
    {
        [JsonPropertyName ("creation_utc")]
        public DateTime? CreationUtc { get; set; }

        [JsonPropertyName ("key")]
        public string? Key { get; set; }

        [JsonPropertyName ("value")]
        public string? Value { get; set; }
    }
}
