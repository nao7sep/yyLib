using System.Text.Json.Serialization;

namespace yyLib
{
    public partial class yyUserSecretsModel
    {
        [JsonPropertyName ("openai")]
        public yyUserSecretsOpenAiModel? OpenAi { get; set; }
    }
}
