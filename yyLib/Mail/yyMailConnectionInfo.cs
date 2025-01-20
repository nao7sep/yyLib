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
        public required string Host { get; set; }

        [JsonPropertyName ("port")]
        public required int Port { get; set; }

        // https://mimekit.net/docs/html/T_MailKit_Security_SecureSocketOptions.htm

        [JsonPropertyName ("secure_socket_options")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        [ConfigurationKeyName ("secure_socket_options")]
        public required SecureSocketOptions SecureSocketOptions { get; set; }

        [JsonPropertyName ("user_name")]
        [ConfigurationKeyName ("user_name")]
        public required string UserName { get; set; }

        [JsonPropertyName ("password")]
        public required string Password { get; set; }

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

            throw new yyInvalidDataException ("No mail connection info found.");
        }

        private static readonly Lazy <yyMailConnectionInfo> _default = new (_CreateDefault ());

        public static yyMailConnectionInfo Default => _default.Value;
    }
}
