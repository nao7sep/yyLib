using System.Text.Json;
using MailKit.Net.Smtp;
using MimeKit;

namespace yyLib
{
    public static class yyMailUtility
    {
        // This method safely reuses the provided SmtpClient instance by ensuring that it always
        // calls ConnectAsync before sending an email and AuthenticateAsync before sending a message.
        // This approach prevents issues related to session timeouts and ensures a fresh connection each time.
        //
        // Why is this safe?
        // - A new connection is established before every email, preventing stale connections.
        // - Authentication is performed each time, avoiding authentication expiration issues.
        // - The method assumes single-threaded use since SmtpClient is not thread-safe.
        //
        // What NOT to do:
        // - Do NOT share the same SmtpClient instance across multiple threads.
        // - Do NOT assume an existing connection is still valid—always call ConnectAsync first.
        // - Do NOT keep the client open indefinitely if not in active use.

        public static async Task <(string JsonString, byte [] MimeBytes, string SendingResult)> SendAsync (
            SmtpClient smtpClient, yyMailConnectionInfo connectionInfo, yyMailMessage message, CancellationToken cancellationToken = default)
        {
            string xJsonString = JsonSerializer.Serialize (message, yyJson.DefaultSerializationOptions);

            using MimeMessage xMimeMessage = new ();
            xMimeMessage.Load (message);

            // Using the default options.
            byte [] xMimeBytes = yyConverter.MimeMessageToBytes (xMimeMessage, cancellationToken: cancellationToken);

            // Connect to the SMTP server using the provided connection info and cancellation token.
            // If the cancellation token is triggered during connection, it cancels the operation and throws OperationCanceledException.
            await smtpClient.ConnectAsync (connectionInfo, cancellationToken).ConfigureAwait (false);

            // Authenticate with the SMTP server using the provided credentials and cancellation token.
            // If the cancellation token is triggered during authentication, it cancels the operation and throws OperationCanceledException.
            await smtpClient.AuthenticateAsync (connectionInfo, cancellationToken).ConfigureAwait (false);

            // Send the MIME message using the SMTP client with the provided cancellation token.
            // If the cancellation token is triggered during sending, it cancels the operation and throws OperationCanceledException.
            string xSendingResult = await smtpClient.SendAsync (xMimeMessage, cancellationToken).ConfigureAwait (false);

            // If any of the above tasks are cancelled due to the cancellation token being triggered,
            // the ongoing operation is stopped immediately and subsequent operations are not initiated.
            // Each cancelled task results in an OperationCanceledException, indicating the task was cancelled.

            return (JsonString: xJsonString, MimeBytes: xMimeBytes, SendingResult: xSendingResult);
        }

        public static async Task <(string JsonString, byte [] MimeBytes, string SendingResult)> SendAsync (
            yyMailConnectionInfo connectionInfo, yyMailMessage message, CancellationToken cancellationToken = default)
        {
            using SmtpClient xSmtpClient = new ()
            {
                // SmtpClient.Timeout is in milliseconds.
                // https://mimekit.net/docs/html/P_MailKit_Net_Smtp_SmtpClient_Timeout.htm
                Timeout = (connectionInfo.Timeout ?? yyMailConnectionInfo.DefaultTimeout) * 1000
            };

            return await SendAsync (xSmtpClient, connectionInfo, message, cancellationToken).ConfigureAwait (false);
        }
    }
}
