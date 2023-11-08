using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyUserSecretsOpenAiModel
    {
        [JsonPropertyName ("api_key")]
        public string? ApiKey { get; set; }
    }
}
