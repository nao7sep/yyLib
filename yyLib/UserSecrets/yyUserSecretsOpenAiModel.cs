using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyUserSecretsOpenAiModel
    {
        [JsonPropertyName ("api_key")]
        public string? ApiKey { get; set; }

        [JsonPropertyName ("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName ("project")]
        public string? Project { get; set; }

        [JsonPropertyName ("chat_endpoint")]
        public string? ChatEndpoint { get; set; }

        [JsonPropertyName ("chat_model")]
        public string? ChatModel { get; set; }

        [JsonPropertyName ("images_endpoint")]
        public string? ImagesEndpoint { get; set; }
    }
}
