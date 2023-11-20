using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyUserSecretsModel
    {
        [JsonPropertyName ("openai")]
        public yyUserSecretsOpenAiModel? OpenAi { get; set; }
    }
}
