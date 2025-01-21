using System.Text;
using yyLib;

namespace yyLibConsole
{
    internal static class MailTest
    {
        public static void Run (string from, string to)
        {
            // One must be provided in user secrets or app settings.
            yyMailConnectionInfo? xConnectionInfo = yyMailConnectionInfo.Default;

            yyMailMessage xMessage = new ();
            (xMessage.From ??= []).Add (new () { Address = from });
            (xMessage.To ??= []).Add (new () { Address = to });
            xMessage.Subject = "Test Message";
            xMessage.TextBody = "This is a test message.";

            var xResult = yyMailUtility.SendAsync (xConnectionInfo!, xMessage).Result;

            string xPartialFilePath = yyPath.Join (yySpecialDirectories.Desktop,
                "MailTest-" + yyConverter.DateTimeToRoundtripFileNameString (DateTime.UtcNow));

            // With JavaScriptEncoder.UnsafeRelaxedJsonEscaping taking effect,
            // the JSON string representation of the message may contain CJK characters.

            string xJsonFilePath = xPartialFilePath + ".json";
            File.WriteAllText (xJsonFilePath, xResult.JsonString, Encoding.UTF8);
            Console.WriteLine ("JSON: " + xJsonFilePath);

            // As the MIME binary representation of the message is generated directly from the message,
            // with or without non-ASCII characters contained, the body of the message seems to be encoded in UTF-8 and so is the entire MIME binary.

            string xMimeFilePath = xPartialFilePath + ".eml";
            File.WriteAllBytes (xMimeFilePath, xResult.MimeBytes);
            Console.WriteLine ("MIME: " + xMimeFilePath);

            Console.WriteLine (xResult.SendingResult);
        }
    }
}
