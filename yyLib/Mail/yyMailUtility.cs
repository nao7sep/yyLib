using System.Text.Json;
using MailKit.Net.Smtp;
using MimeKit;

namespace yyLib
{
    public static class yyMailUtility
    {
        public static async Task <(string JsonString, byte [] MimeBytes, string SendingResult)> SendAsync (
            yyMailConnectionInfo connectionInfo, yyMailMessage message, CancellationToken cancellationToken = default)
        {
            string xJsonString = JsonSerializer.Serialize (message, yyJson.DefaultSerializationOptions);

            using MimeMessage xMimeMessage = new ();
            xMimeMessage.Load (message);

            // Using the default options.
            byte [] xMimeBytes = yyConvertor.MimeMessageToBytes (xMimeMessage, cancellationToken: cancellationToken);

            using SmtpClient xSmtpClient = new ();

            // Added comments by ChatGPT:

            // Connect to the SMTP server using the provided connection info and cancellation token.
            // If the cancellation token is triggered during connection, it cancels the operation and throws OperationCanceledException.
            await xSmtpClient.ConnectAsync (connectionInfo, cancellationToken);

            // Authenticate with the SMTP server using the provided credentials and cancellation token.
            // If the cancellation token is triggered during authentication, it cancels the operation and throws OperationCanceledException.
            await xSmtpClient.AuthenticateAsync (connectionInfo, cancellationToken);

            // Send the MIME message using the SMTP client with the provided cancellation token.
            // If the cancellation token is triggered during sending, it cancels the operation and throws OperationCanceledException.
            string xSendingResult = await xSmtpClient.SendAsync (xMimeMessage, cancellationToken);

            // If any of the above tasks are cancelled due to the cancellation token being triggered,
            // the ongoing operation is stopped immediately and subsequent operations are not initiated.
            // Each cancelled task results in an OperationCanceledException, indicating the task was cancelled.

            return (JsonString: xJsonString, MimeBytes: xMimeBytes, SendingResult: xSendingResult);
        }
    }
}
