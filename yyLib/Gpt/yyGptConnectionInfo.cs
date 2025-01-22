using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyGptConnectionInfo
    {
        // These properties are intentionally left optional to support fallback behavior.
        // If an instance of a derived class does not populate all required values,
        // the default instance of the base class will provide fallback data.
        // This approach avoids JSON deserialization exceptions that would occur if
        // properties were marked as required and values were missing during deserialization.
        // Attempts to use DefaultValue or setting default values directly in the property definition
        // did not prevent the exceptions, so this design ensures smooth functionality.

        [JsonPropertyName ("api_key")]
        [ConfigurationKeyName ("api_key")]
        public string? ApiKey { get; set; }

        [JsonPropertyName ("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName ("project")]
        public string? Project { get; set; }

        [JsonPropertyName ("endpoint")]
        public string? Endpoint { get; set; }

        [JsonPropertyName ("timeout")]
        public int? Timeout { get; set; }

        // -----------------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------------

        // In C#, HttpClient only provides a single Timeout property, which applies to the entire HTTP operation.
        // This includes connection establishment, data transfer, and response time. Unlike Python's HTTPX library,
        // which allows fine-grained timeout settings for specific operations (e.g., connect, read, write, pool),
        // C# does not offer per-call timeout granularity.
        // For reference on HTTPX timeouts: https://www.python-httpx.org/advanced/timeouts/
        //
        // Example from HTTPX:
        //   connect=5s, read=10s, write=5s, pool=15s
        //
        // HttpClient methods such as GetAsync and PostAsync rely on this single Timeout property,
        // meaning you cannot individually configure timeouts for each part of the HTTP request lifecycle.
        //
        // Regarding default timeout values:
        // - HTTPX has a default timeout of 5 seconds, which is often too short for many real-world scenarios
        //   where network latency and response times can vary significantly.
        // - HttpClient, on the other hand, defaults to 100 seconds, which can be excessively long in cases
        //   where operations are expected to fail quickly.
        //
        // This model class provides a `Timeout` property that allows customization of this behavior.
        // Additionally, each derived class can define its own DefaultTimeout value. Developers should rely
        // on the most specific default available when falling back to defaults in their code.
        // For example:
        //   - Code working with `yyGptChatConnectionInfo` should use `yyGptChatConnectionInfo.DefaultTimeout`.
        //   - Avoid directly using the base class default (`yyGptConnectionInfo.DefaultTimeout`) unless no
        //     more specific value is available in the derived class.

        public static readonly int DefaultTimeout = 30;

        // Suppresses the "CA1707: Identifiers should not contain underscores" rule from code analysis tools.
        [SuppressMessage ("Naming", "CA1707")]
        protected static ConnectionInfoType _CopyMissingValues <ConnectionInfoType> (
            ConnectionInfoType target, yyGptConnectionInfo source) where ConnectionInfoType: yyGptConnectionInfo
        {
            if (string.IsNullOrWhiteSpace (target.ApiKey))
                target.ApiKey = source.ApiKey;

            if (string.IsNullOrWhiteSpace (target.Organization))
                target.Organization = source.Organization;

            if (string.IsNullOrWhiteSpace (target.Project))
                target.Project = source.Project;

            if (string.IsNullOrWhiteSpace (target.Endpoint))
                target.Endpoint = source.Endpoint;

            if (target.Timeout.HasValue == false)
                target.Timeout = source.Timeout;

            return target;
        }

        private static yyGptConnectionInfo _CreateDefault ()
        {
            var xGptConnectionSection = yyAppSettings.Config.GetSection ("gpt_connection");

            if (xGptConnectionSection.Exists () &&
                xGptConnectionSection.GetChildren ().Any () &&
                xGptConnectionSection.Get <yyGptConnectionInfo> () is { } xGptConnectionInfo)
                    return xGptConnectionInfo;

            if (yyUserSecrets.Default.GptConnection != null)
                return yyUserSecrets.Default.GptConnection;

            return new ()
            {
                Timeout = DefaultTimeout
            };
        }

        private static readonly Lazy <yyGptConnectionInfo> _default = new (() => _CreateDefault ());

        public static yyGptConnectionInfo Default => _default.Value;
    }
}
