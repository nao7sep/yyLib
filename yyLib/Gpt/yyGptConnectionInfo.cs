using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyGptConnectionInfo
    {
        [JsonPropertyName ("api_key")]
        [ConfigurationKeyName ("api_key")]
        public required string ApiKey { get; set; }

        [JsonPropertyName ("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName ("project")]
        public string? Project { get; set; }

        // The Endpoint property in this class is required and does not have a default value.
        // This design ensures that users explicitly specify the correct endpoint, preventing accidental connections to unintended services.
        // For instance, OpenAI and Azure OpenAI Service have different endpoints and configurations.
        // OpenAI's endpoint might look like "https://api.openai.com/v1/...", whereas Azure OpenAI Service uses a format like "https://{your-resource-name}.openai.azure.com/openai/deployments/{deployment-id}/...".
        // Requiring explicit endpoint specification promotes clarity and reduces the risk of misconfiguration.
        // Reference: https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/switching-endpoints
        [JsonPropertyName ("endpoint")]
        public required string Endpoint { get; set; }

        [JsonPropertyName ("timeout")]
        public required int Timeout { get; set; }

        // -----------------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------------

        // Unlike Python's "HTTPX", C#'s HttpClient only provides a single Timeout property
        // that applies to the entire HTTP operation (e.g., connection, data transfer, and response time).
        // In Python's HTTPX, timeouts can be set separately for connect, read, write, and pool operations.
        // For example, HTTPX allows: connect=5s, read=10s, write=5s, pool=15s.
        // C# HttpClient methods (e.g., GetAsync, PostAsync) do not support specifying per-call timeout values.
        // Reference: https://www.python-httpx.org/advanced/timeouts/

        // The Timeout property in this class is required and does not initialize to any default value.
        // It is not nullable, meaning the user must explicitly set a value for the connection to function.
        // This approach emphasizes transparency and ensures that the timeout duration is a deliberate choice.
        // How long a connection waits before failing is critical to user experience and shouldn't be hidden
        // behind implicit behavior. This design encourages clarity and better control over timeout settings.

        // HTTPX's default timeout of 5 seconds is often too short for many real-world scenarios, where network
        // latency and response times may vary. On the other hand, HttpClient's default of 100 seconds can be
        // excessively long and may leave users waiting unnecessarily for a failure. If the Timeout property
        // were made nullable and fell back to an implicit value, users might not know whether to expect a
        // quick response or tolerate delays, leading to confusion and degraded user experience.

        // By default, HttpClient's Timeout property is set to 100 seconds.
        // However, in some scenarios, such as when interacting with APIs like OpenAI's,
        // a lower timeout is often more appropriate to improve responsiveness and reliability.

        public static readonly int DefaultTimeout = 30;

        private static yyGptConnectionInfo _CreateDefault ()
        {
            var xGptConnectionSection = yyAppSettings.Config.GetSection ("gpt_connection");

            if (xGptConnectionSection.Exists () &&
                xGptConnectionSection.GetChildren ().Any () &&
                xGptConnectionSection.Get <yyGptConnectionInfo> () is { } xGptConnectionInfo)
                    return xGptConnectionInfo;

            if (yyUserSecrets.Default.GptConnection != null)
                return yyUserSecrets.Default.GptConnection;

            throw new yyInvalidDataException ("No GPT connection info found.");
        }

        private static readonly Lazy <yyGptConnectionInfo> _default = new (() => _CreateDefault ());

        public static yyGptConnectionInfo Default => _default.Value;
    }
}
