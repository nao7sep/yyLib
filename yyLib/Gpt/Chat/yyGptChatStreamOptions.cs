using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatStreamOptions
    {
        [JsonPropertyName ("include_usage")]
        public bool? IncludeUsage { get; set; }
    }
}
