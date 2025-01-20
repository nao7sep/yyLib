using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyLog
    {
        [JsonPropertyName ("created_at_utc")]
        public required DateTime CreatedAtUtc { get; set; }

        [JsonPropertyName ("key")]
        public required string Key { get; set; }

        [JsonPropertyName ("value")]
        public required string Value { get; set; }
    }
}
