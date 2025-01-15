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

        [JsonPropertyName ("secure_socket_options")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public SecureSocketOptions? SecureSocketOptions { get; set; }

        [JsonPropertyName ("user_name")]
        public string? UserName { get; set; }

        [JsonPropertyName ("password")]
        public string? Password { get; set; }

        // -----------------------------------------------------------------------------
        // Default Connection
        // -----------------------------------------------------------------------------

        private static yyMailConnectionInfo _CreateDefault ()
        {
            if (yyAppSettings.Config.GetSection ("mail_connection").Get <yyMailConnectionInfo> () is { } xMailConnection)
                return xMailConnection;

            if (yyUserSecrets.Default.MailConnection != null)
                return yyUserSecrets.Default.MailConnection;

            return new yyMailConnectionInfo ();
        }

        private static readonly Lazy <yyMailConnectionInfo> _default = new (_CreateDefault ());

        public static yyMailConnectionInfo Default => _default.Value;
    }
}
