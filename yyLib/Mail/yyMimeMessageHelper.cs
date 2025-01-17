using MimeKit;

namespace yyLib
{
    public static class yyMimeMessageHelper
    {
        public static void Load (this MimeMessage mimeMessage, yyMailMessage mailMessage)
        {
            if (mailMessage.Bcc != null)
                mimeMessage.Bcc.AddRange (mailMessage.Bcc.Select (x => new MailboxAddress (x.Name, x.Address)));

            if (mailMessage.Cc != null)
                mimeMessage.Cc.AddRange (mailMessage.Cc.Select (x => new MailboxAddress (x.Name, x.Address)));

            if (mailMessage.DateUtc != null)
                mimeMessage.Date = mailMessage.DateUtc.Value;

            if (mailMessage.From != null)
                mimeMessage.From.AddRange (mailMessage.From.Select (x => new MailboxAddress (x.Name, x.Address)));

            if (mailMessage.Headers != null)
            {
                foreach (var xHeader in mailMessage.Headers)
                    mimeMessage.Headers.Add (xHeader.Key, xHeader.Value);
            }

            if (mailMessage.Importance != null)
                mimeMessage.Importance = mailMessage.Importance.Value;

            if (mailMessage.InReplyTo != null)
                mimeMessage.InReplyTo = mailMessage.InReplyTo;

            if (mailMessage.MessageId != null)
                mimeMessage.MessageId = mailMessage.MessageId;

            if (mailMessage.MimeVersion != null)
                mimeMessage.MimeVersion = mailMessage.MimeVersion;

            if (mailMessage.Priority != null)
                mimeMessage.Priority = mailMessage.Priority.Value;

            if (mailMessage.References != null)
                mimeMessage.References.AddRange (mailMessage.References);

            if (mailMessage.ReplyTo != null)
                mimeMessage.ReplyTo.AddRange (mailMessage.ReplyTo.Select (x => new MailboxAddress (x.Name, x.Address)));

            if (mailMessage.Sender != null)
                mimeMessage.Sender = new MailboxAddress (mailMessage.Sender.Name, mailMessage.Sender.Address);

            if (mailMessage.Subject != null)
                mimeMessage.Subject = mailMessage.Subject;

            if (mailMessage.To != null)
                mimeMessage.To.AddRange (mailMessage.To.Select (x => new MailboxAddress (x.Name, x.Address)));

            if (mailMessage.XPriority != null)
                mimeMessage.XPriority = mailMessage.XPriority.Value;

            // If there's nothing at all to output, BodyBuilder creates a message with "Content-Type: text/plain; charset=utf-8" and no content.
            // We could also say an empty string is the mail body.

            // If there's one, we get a single part message with the right "Content-Type".

            // If there are 2 or more, we get a multipart message with the right "Content-Type" and a "boundary" parameter.

            // So, no reason to check the actual number of parts or determine whether to update message.Body or not.

            BodyBuilder xBodyBuilder = new ();

            if (string.IsNullOrWhiteSpace (mailMessage.HtmlBody) == false)
                xBodyBuilder.HtmlBody = mailMessage.HtmlBody;

            if (string.IsNullOrWhiteSpace (mailMessage.TextBody) == false)
                xBodyBuilder.TextBody = mailMessage.TextBody;

            if (mailMessage.Attachments != null)
            {
                foreach (var xAttachment in mailMessage.Attachments)
                {
                    xBodyBuilder.Attachments.Add (new MimePart (MimeTypes.GetMimeType (xAttachment.OriginalFilePath))
                    {
                        FileName = xAttachment.NewFileName ?? Path.GetFileName (xAttachment.OriginalFilePath),
                        ContentDisposition = new ContentDisposition (ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        Content = new MimeContent (new MemoryStream (File.ReadAllBytes (xAttachment.OriginalFilePath!))) // Avoids memory leakage.
                    });
                }
            }

            mimeMessage.Body = xBodyBuilder.ToMessageBody ();
        }
    }
}
