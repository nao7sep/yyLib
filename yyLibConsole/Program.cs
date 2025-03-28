using yyLib;

namespace yyLibConsole
{
    internal static class Program
    {
        internal static void Main (string [] args)
        {
            try
            {
            }

            catch (Exception xException)
            {
                yyLogger.Default.TryWriteException (xException);
                yyConsole.WriteErrorLine (xException.ToString ());
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
