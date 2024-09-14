using System.Text.Json.Serialization;
using MailKit.Security;

namespace yyMailLib
{
    public class yyMailConnectionInfo
    {
        // Minimal info to use the following class:
        // http://www.mimekit.net/docs/html/T_MailKit_Net_Smtp_SmtpClient.htm

        [JsonPropertyName ("host")]
        public string? Host { get; set; }

        [JsonPropertyName ("port")]
        public int? Port { get; set; }

        /// <summary>
        /// Set to SslOnConnect by default for security reasons.
        /// </summary>
        [JsonPropertyName ("secure_socket_options")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public SecureSocketOptions? SecureSocketOptions { get; set; } = MailKit.Security.SecureSocketOptions.SslOnConnect;

        [JsonPropertyName ("user_name")]
        public string? UserName { get; set; }

        [JsonPropertyName ("password")]
        public string? Password { get; set; }
    }
}
