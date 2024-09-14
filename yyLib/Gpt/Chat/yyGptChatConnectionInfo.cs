using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptChatConnectionInfo
    {
        [JsonPropertyName ("api_key")]
        public string? ApiKey { get; set; }

        [JsonPropertyName ("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName ("project")]
        public string? Project { get; set; }

        [JsonPropertyName ("endpoint")]
        public string? Endpoint { get; set; }

        [JsonPropertyName ("timeout")]
        public int? Timeout { get; set; }

        public yyGptChatConnectionInfo ()
        {
            ApiKey = yyGpt.DefaultApiKey;
            Organization = yyGpt.DefaultOrganization;
            Project = yyGpt.DefaultProject;
            Endpoint = yyGptChat.DefaultEndpoint;
            Timeout = yyGpt.DefaultTimeout;
        }
    }
}
