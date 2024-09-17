using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptImagesConnectionInfo
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

        public yyGptImagesConnectionInfo ()
        {
            ApiKey = yyGpt.DefaultApiKey;
            Organization = yyGpt.DefaultOrganization;
            Project = yyGpt.DefaultProject;
            Endpoint = yyGptImages.DefaultEndpoint;
            Timeout = yyGpt.DefaultTimeout;
        }
    }
}
