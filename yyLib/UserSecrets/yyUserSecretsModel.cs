using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyUserSecretsModel
    {
        private static readonly Lazy <yyUserSecretsModel> _default = new (() => yyUserSecretsLoader.Load (yyUserSecrets.DefaultFilePaths));

        /// <summary>
        /// NOT thread-safe.
        /// </summary>
        public static yyUserSecretsModel Default => _default.Value;

        [JsonPropertyName ("openai")]
        public yyUserSecretsOpenAiModel? OpenAi { get; set; }
    }
}
