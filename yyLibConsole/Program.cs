using yyLib;

namespace yyLibConsole
{
    internal class Program
    {
        static void Main (string [] args)
        {
            try
            {
                // API key must be specified in user secrets or app settings.
                // GptTest.Run (
                //     firstAssistantSystemMessage: "You always say a random word.",
                //     secondAssistantSystemMessage: "You always make a random sentence using the provided word.",
                //     interactionCount: 10);

                // Set "from" and "to" email addresses for testing.
                // Default connection must be specified in user secrets or app settings.
                // MailTest.Run (from: string.Empty, to: string.Empty);
            }

            catch (Exception xException)
            {
                yyLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException.ToString ());
            }

            finally
            {
                Console.Write ("Press any key to exit: ");
                Console.ReadKey (true);
                Console.WriteLine ();
            }
        }
    }
}
