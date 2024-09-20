using System.Text.Json;
using MailKit.Net.Smtp;
using MimeKit;

namespace yyLib
{
    public static class yyMailUtility
    {
        public static async Task <(string JsonString, string MimeString, string SendingResult)> SendAsync (yyMailConnectionInfo connectionInfo, yyMailMessage message)
        {
            string xJsonString = JsonSerializer.Serialize (message, yyJson.DefaultSerializationOptions);

            using MimeMessage xMimeMessage = new ();
            xMimeMessage.Load (message);
            string xMimeString = xMimeMessage.ToString ();

            using SmtpClient xSmtpClient = new ();
            await xSmtpClient.ConnectAsync (connectionInfo);
            await xSmtpClient.AuthenticateAsync (connectionInfo);
            string xSendingResult = await xSmtpClient.SendAsync (xMimeMessage);

            return (JsonString: xJsonString, MimeString: xMimeString, SendingResult: xSendingResult);
        }
    }
}
