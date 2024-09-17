using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyLog
    {
        [JsonPropertyName ("created_at_utc")]
        public DateTime? CreatedAtUtc { get; set; }

        [JsonPropertyName ("key")]
        public string? Key { get; set; }

        [JsonPropertyName ("value")]
        public string? Value { get; set; }
    }
}
