using yyLib;

namespace yyLibConsole
{
    internal class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter
        static void Main (string [] args)
#pragma warning restore IDE0060
        {
            try
            {
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException);
            }
        }
    }
}
