using System.Text;
using System.Text.Json;
using MailKit.Net.Smtp;
using MimeKit;
using yyLib;
using yyMailLib;

namespace yyMailLibConsole
{
    internal class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter
        static async Task Main (string [] args)
#pragma warning restore IDE0060
        {
            try
            {
                bool xContinues = true;

                // -----------------------------------------------------------------------------

                string xConnectionInfoFilePath = yyAppDirectory.MapPath ("ConnectionInfo.json");
                yyMailConnectionInfo? xConnectionInfo = null;

                if (File.Exists (xConnectionInfoFilePath) == false)
                {
                    // Makes a temp file and exits.

                    var xTempConnectionInfo = new yyMailConnectionInfo
                    {
                        Host = "HOST",

                        // ChatGPT says:

                            // SMTP Standard Ports:
                            // 1. Port 25: Default SMTP non-encrypted port. Often blocked by ISPs to prevent spam.
                            // 2. Port 587: Default port for SMTP submissions. Used for sending emails from clients to servers. Supports STARTTLS for encrypted communication.
                            // 3. Port 465: Historically used for SMTPS (SMTP over SSL), and now revived for SMTP submissions over SSL for secure communication from the start.

                        Port = 587,

                        // xConnectionInfo.SecureSocketOptions is SecureSocketOptions.SslOnConnect by default.

                        UserName = "USERNAME",
                        Password = "PASSWORD"
                    };

                    string xJson = JsonSerializer.Serialize (xTempConnectionInfo, yyJson.DefaultSerializationOptions);

                    // It is generally recommended not to insert a BOM nowadays,
                    // but I, as a CJK-region developer, am very happy going around inserting them everywhere.
                    File.WriteAllText (xConnectionInfoFilePath, xJson, Encoding.UTF8);

                    Console.WriteLine ("Please edit 'ConnectionInfo.json' and run this program again.");
                    xContinues = false;
                }

                else
                {
                    string xFileContents = File.ReadAllText (xConnectionInfoFilePath, Encoding.UTF8);
                    xConnectionInfo = JsonSerializer.Deserialize <yyMailConnectionInfo> (xFileContents, yyJson.DefaultDeserializationOptions);
                    Console.WriteLine (JsonSerializer.Serialize (xConnectionInfo, yyJson.DefaultSerializationOptions));
                }

                // -----------------------------------------------------------------------------

                // Let's do the same thing with the sender, who'll be added to From.

                string xSenderInfoFilePath = yyAppDirectory.MapPath ("Sender.json");
                yyMailContact? xSenderInfo = null;

                if (File.Exists (xSenderInfoFilePath) == false)
                {
                    var xTempSenderInfo = new yyMailContact
                    {
                        Name = "NAME",
                        Address = "ADDRESS"
                    };

                    string xJson = JsonSerializer.Serialize (xTempSenderInfo, yyJson.DefaultSerializationOptions);
                    File.WriteAllText (xSenderInfoFilePath, xJson, Encoding.UTF8);
                    Console.WriteLine ("Please edit 'Sender.json' and run this program again.");
                    xContinues = false;
                }

                else
                {
                    string xFileContents = File.ReadAllText (xSenderInfoFilePath, Encoding.UTF8);
                    xSenderInfo = JsonSerializer.Deserialize <yyMailContact> (xFileContents, yyJson.DefaultDeserializationOptions);
                    Console.WriteLine (JsonSerializer.Serialize (xSenderInfo, yyJson.DefaultSerializationOptions));
                }

                // -----------------------------------------------------------------------------

                string xRecipientInfoFilePath = yyAppDirectory.MapPath ("Recipient.json");
                yyMailContact? xRecipientInfo = null;

                if (File.Exists (xRecipientInfoFilePath) == false)
                {
                    var xTempRecipientInfo = new yyMailContact
                    {
                        Name = "NAME",
                        Address = "ADDRESS"
                    };

                    // Roundtrip test:
                    // These values arent used for the test.

                    xTempRecipientInfo.AddPreferredLanguage ("Japanese");
                    xTempRecipientInfo.AddPreferredLanguage ("Chinese");
                    xTempRecipientInfo.PreferredBodyFormat = yyMailMessageBodyFormat.Plaintext;

                    string xJson = JsonSerializer.Serialize (xTempRecipientInfo, yyJson.DefaultSerializationOptions);
                    File.WriteAllText (xRecipientInfoFilePath, xJson, Encoding.UTF8);
                    Console.WriteLine ("Please edit 'Recipient.json' and run this program again.");
                    xContinues = false;
                }

                else
                {
                    string xFileContents = File.ReadAllText (xRecipientInfoFilePath, Encoding.UTF8);
                    xRecipientInfo = JsonSerializer.Deserialize <yyMailContact> (xFileContents, yyJson.DefaultDeserializationOptions);
                    Console.WriteLine (JsonSerializer.Serialize (xRecipientInfo, yyJson.DefaultSerializationOptions));
                }

                // -----------------------------------------------------------------------------

                // I wont be doing "Press any key to..."
                // We usually run this program with a debugger attached.

                if (xContinues == false)
                    return;

                // -----------------------------------------------------------------------------

                // Generates something like: f63d949c5a894e5b8a1157ce55fe4382@6feceaa5c9f844dea273b1b142b99050
                static string GenerateRandomMailAddress () => $"{Guid.NewGuid ():N}@{Guid.NewGuid ():N}";

                // Message IDs are automatically wrapped in angle brackets.
                static string GenerateRandomMessageId () =>  Guid.NewGuid ().ToString ("N");

                // -----------------------------------------------------------------------------

                yyMailMessage xMessage = new ();

                // In alphabetical order, excluding large files.

                foreach (string xFilePath in Directory.GetFiles (yyAppDirectory.Path, "*.*", SearchOption.TopDirectoryOnly).
                        Where (x => new FileInfo (x).Length < 100 * 1024).OrderBy (y => y, StringComparer.OrdinalIgnoreCase))
                    xMessage.AddAttachment (xFilePath);

                xMessage.AddBcc (GenerateRandomMailAddress ());
                xMessage.AddBcc (GenerateRandomMailAddress ());

                xMessage.AddCc (GenerateRandomMailAddress ());
                xMessage.AddCc (GenerateRandomMailAddress ());

                xMessage.DateUtc = DateTime.UtcNow; // Roundtrip successful.

                xMessage.AddFrom (xSenderInfo!);
                xMessage.AddFrom (GenerateRandomMailAddress ());

                xMessage.AddHeader ("Test1", "test1");
                xMessage.AddHeader ("Test2", "test2");

                xMessage.HtmlBody = "<b>HTML body.</b>";

                xMessage.AddHtmlBodyTranslation (new yyMailMessageTranslation
                {
                    Utc = DateTime.UtcNow,
                    Language = "Japanese",
                    Text = "<b>HTML の本文。</b>",

                    Details = new Dictionary <string, string>
                    {
                        { "Translator", "MySleepyHead" },
                        { "Fee", "Free" }
                    }
                });

                xMessage.Importance = MessageImportance.Low;

                xMessage.InReplyTo = GenerateRandomMessageId ();

                xMessage.Language = "English";

                xMessage.MessageId = GenerateRandomMessageId ();

                // ChatGPT says:

                    // MIME (Multipurpose Internet Mail Extensions) Versions:

                    // MIME Version 1.0:
                    // - Introduced in 1991 and detailed in RFC 1341 and RFC 1521.
                    // - This was the initial version of MIME and laid the foundation for the format
                    //   of email messages supporting content types beyond simple text,
                    //   including attachments of audio, video, images, and applications.

                    // MIME Version 2.0:
                    // - MIME Version 2.0 is not formally recognized as a distinct version.
                    // - Instead, various updates and enhancements to MIME Version 1.0 have been made
                    //   through additional RFCs (Requests for Comments).
                    // - These updates expanded and refined the MIME standards but did not consolidate
                    //   into a formal "Version 2.0."
                    // - These updates ensure backward compatibility and have been part of continued
                    //   evolution of the MIME standards.

                    // Note:
                    // - The primary reference point remains MIME Version 1.0, with subsequent RFCs
                    //   (such as RFC 2045 to RFC 2049, which are part of MIME Part One: Format of
                    //   Internet Message Bodies) providing updates and clarifications.

                xMessage.MimeVersion = new Version (2, 0);

                xMessage.Priority = MessagePriority.NonUrgent;

                xMessage.AddReference (GenerateRandomMessageId ());
                xMessage.AddReference (GenerateRandomMessageId ());

                xMessage.AddReplyTo (GenerateRandomMailAddress ());
                xMessage.AddReplyTo (GenerateRandomMailAddress ());

                xMessage.Sender = xSenderInfo!;

                xMessage.Subject = "Subject";

                xMessage.AddSubjectTranslation (new yyMailMessageTranslation
                {
                    Utc = DateTime.UtcNow,
                    Language = "Japanese",
                    Text = "件名",

                    Details = new Dictionary <string, string>
                    {
                        { "Translator", "MySleepyHead" },
                        { "Fee", "Free" }
                    }
                });

                xMessage.TextBody = "Plaintext body.";

                xMessage.AddTextBodyTranslation (new yyMailMessageTranslation
                {
                    Utc = DateTime.UtcNow,
                    Language = "Japanese",
                    Text = "プレーンテキストの本文。",

                    Details = new Dictionary <string, string>
                    {
                        { "Translator", "MySleepyHead" },
                        { "Fee", "Free" }
                    }
                });

                xMessage.AddTo (xRecipientInfo!);
                xMessage.AddTo (GenerateRandomMailAddress ());

                xMessage.XPriority = XMessagePriority.Low;

                string xJsonAlt = JsonSerializer.Serialize (xMessage, yyJson.DefaultSerializationOptions);
                Console.WriteLine (xJsonAlt);

                // -----------------------------------------------------------------------------

                string xPartialFilePath = Path.Join (yyAppDirectory.MapPath ("Messages"),
                    $"Message-{DateTime.Now:yyyyMMdd'-'HHmmss}"); // Local time.

                yyDirectory.CreateParent (xPartialFilePath);

                // Should be safe enough.
                File.WriteAllText (xPartialFilePath + ".json", xJsonAlt, Encoding.UTF8);

                using MimeMessage xMimeMessage = new ();
                xMimeMessage.Load (xMessage);
                File.WriteAllText (xPartialFilePath + ".eml", xMimeMessage.ToString (), Encoding.UTF8);

                // -----------------------------------------------------------------------------

                using SmtpClient xMailClient = new ();

                await xMailClient.ConnectAsync (xConnectionInfo!);
                await xMailClient.AuthenticateAsync (xConnectionInfo!);

                var xSendingTask = await xMailClient.SendAsync (xMimeMessage);

                Console.WriteLine (xSendingTask.GetVisibleString ());
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ());
            }
        }
    }
}
