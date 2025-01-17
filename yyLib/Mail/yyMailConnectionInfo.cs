using System.Text.Json.Serialization;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyMailConnectionInfo
    {
        // Minimal info to use the following class:
        // http://www.mimekit.net/docs/html/T_MailKit_Net_Smtp_SmtpClient.htm

        [JsonPropertyName ("host")]
        public string? Host { get; set; }

        [JsonPropertyName ("port")]
        public int? Port { get; set; }

        // I'll be setting the ConfigurationKeyName attribute ONLY to the properties that would, otherwise, fail to receive values from IConfiguration's default binder.
        // For now, the binding is one-way only from IConfiguration to model classes, and the binder's default behavior seems to be case-insensitive.

        [JsonPropertyName ("secure_socket_options")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        [ConfigurationKeyName ("secure_socket_options")]
        public SecureSocketOptions? SecureSocketOptions { get; set; }

        [JsonPropertyName ("user_name")]
        [ConfigurationKeyName ("user_name")]
        public string? UserName { get; set; }

        [JsonPropertyName ("password")]
        public string? Password { get; set; }

        // -----------------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------------

        private static yyMailConnectionInfo _CreateDefault ()
        {
            var xMailConnectionSection = yyAppSettings.Config.GetSection ("mail_connection");

            if (xMailConnectionSection.Exists () &&
                xMailConnectionSection.GetChildren ().Any () &&
                xMailConnectionSection.Get <yyMailConnectionInfo> () is { } xMailConnection)
                    return xMailConnection;

            if (yyUserSecrets.Default.MailConnection != null)
                return yyUserSecrets.Default.MailConnection;

            return new yyMailConnectionInfo ();
        }

        private static readonly Lazy <yyMailConnectionInfo> _default = new (_CreateDefault ());

        public static yyMailConnectionInfo Default => _default.Value;
    }
}
