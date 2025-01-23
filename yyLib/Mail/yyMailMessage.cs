using System.Text.Json.Serialization;
using MimeKit;

namespace yyLib
{
    public class yyMailMessage
    {
        // Basically, everything that is not Resent* from the following page:
        // http://www.mimekit.net/docs/html/T_MimeKit_MimeMessage.htm

        // Regarding encoding settings:

        // In MimeKit, there's generally no need to explicitly set UTF-8 encoding for standard email components
        // like the subject line, body, or address headers. MimeKit is engineered to automatically handle these encodings.

        // For the subject line and headers, MimeKit encodes them using 'encoded-word' syntax (RFC 2047)
        // if they contain non-ASCII characters. This encoding is done in either 'B' encoding (akin to Base64)
        // or 'Q' encoding (similar to Quoted-Printable), chosen based on efficiency for the given text.

        // When creating a TextPart for the email body and assigning text to it, MimeKit defaults to UTF-8 encoding
        // if the text includes non-ASCII characters. This applies to both plain text and HTML content.
        // Therefore, there's no need to manually specify UTF-8 encoding for these parts.

        // Address fields, such as 'From', 'To', and 'Cc', are also automatically handled by MimeKit.
        // If the display name in these fields has non-ASCII characters, MimeKit encodes them appropriately.

        // For attachments and other binary parts of the email, the encoding is more about how the binary data
        // is transferred (like Base64) rather than character encoding (UTF-8).

        // Overall, MimeKit's default behavior efficiently manages necessary encodings, including UTF-8 where appropriate,
        // reducing the need for manual settings or interventions in the encoding process.

        [JsonPropertyName ("attachments")]
        public IList <yyMailAttachment>? Attachments { get; set; }

        public void AddAttachment (yyMailAttachment attachment) => (Attachments ??= []).Add (attachment);

        public void AddAttachment (string originalFilePath, string? newFileName = null) => AddAttachment (new ()
        {
            OriginalFilePath = originalFilePath,
            NewFileName = newFileName
        });

        [JsonPropertyName ("bcc")]
        public IList <yyMailContact>? Bcc { get; set; }

        public void AddBcc (yyMailContact contact) => (Bcc ??= []).Add (contact);

        public void AddBcc (string address, string? name = null) => AddBcc (new ()
        {
            Address = address,
            Name = name
        });

        [JsonPropertyName ("cc")]
        public IList <yyMailContact>? Cc { get; set; }

        public void AddCc (yyMailContact contact) => (Cc ??= []).Add (contact);

        public void AddCc (string address, string? name = null) => AddCc (new ()
        {
            Address = address,
            Name = name
        });

        // The name may appear redundant.
        // Just making sure.
        // Everything will be hand-copied to MimeMessage.

        [JsonPropertyName ("date_utc")]
        public DateTime? DateUtc { get; set; }

        // http://www.mimekit.net/docs/html/P_MimeKit_MimeMessage_From.htm

        /// <summary>
        /// Sender must be the actual person sending the message if From contains multiple people.
        /// </summary>
        [JsonPropertyName ("from")]
        public IList <yyMailContact>? From { get; set; }

        public void AddFrom (yyMailContact contact) => (From ??= []).Add (contact);

        public void AddFrom (string address, string? name = null) => AddFrom (new ()
        {
            Address = address,
            Name = name
        });

        [JsonPropertyName ("headers")]
        public IDictionary <string, string>? Headers { get; set; }

        public void AddHeader (string key, string value) => (Headers ??= new Dictionary <string, string> ()).Add (key, value);

        [JsonPropertyName ("html_body")]
        public string? HtmlBody { get; set; }

        [JsonPropertyName ("html_body_translations")]
        public IList <yyMailTranslation>? HtmlBodyTranslations { get; set; }

        public void AddHtmlBodyTranslation (yyMailTranslation translation) => (HtmlBodyTranslations ??= []).Add (translation);

        [JsonPropertyName ("importance")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public MessageImportance? Importance { get; set; }

        [JsonPropertyName ("in_reply_to")]
        public string? InReplyTo { get; set; }

        [JsonPropertyName ("language")]
        public string? Language { get; set; }

        [JsonPropertyName ("message_id")]
        public string? MessageId { get; set; }

        [JsonPropertyName ("mime_version")]
        public Version? MimeVersion { get; set; }

        [JsonPropertyName ("priority")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public MessagePriority? Priority { get; set; }

        [JsonPropertyName ("references")]
        public IList <string>? References { get; set; }

        public void AddReference (string reference) => (References ??= []).Add (reference);

        [JsonPropertyName ("reply_to")]
        public IList <yyMailContact>? ReplyTo { get; set; }

        public void AddReplyTo (yyMailContact contact) => (ReplyTo ??= []).Add (contact);

        public void AddReplyTo (string address, string? name = null) => AddReplyTo (new ()
        {
            Address = address,
            Name = name
        });

        [JsonPropertyName ("sender")]
        public yyMailContact? Sender { get; set; }

        [JsonPropertyName ("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName ("subject_translations")]
        public IList <yyMailTranslation>? SubjectTranslations { get; set; }

        public void AddSubjectTranslation (yyMailTranslation translation) => (SubjectTranslations ??= []).Add (translation);

        [JsonPropertyName ("text_body")]
        public string? TextBody { get; set; }

        [JsonPropertyName ("text_body_translations")]
        public IList <yyMailTranslation>? TextBodyTranslations { get; set; }

        public void AddTextBodyTranslation (yyMailTranslation translation) => (TextBodyTranslations ??= []).Add (translation);

        [JsonPropertyName ("to")]
        public IList <yyMailContact>? To { get; set; }

        public void AddTo (yyMailContact contact) => (To ??= []).Add (contact);

        public void AddTo (string address, string? name = null) => AddTo (new ()
        {
            Address = address,
            Name = name
        });

        [JsonPropertyName ("x_priority")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public XMessagePriority? XPriority { get; set; }
    }
}
