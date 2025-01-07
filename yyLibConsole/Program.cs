using yyLib;

namespace yyLibConsole
{
    internal class Program
    {
        static void Main (string [] args)
        {
            try
            {
                // Set "from" and "to" email addresses for testing.
                // Default connection must be specified in user secrets.
                // MailTest.Run (from: "", to: "");
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
