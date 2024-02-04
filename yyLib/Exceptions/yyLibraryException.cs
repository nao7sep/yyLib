namespace yyLib
{
    public class yyLibraryException: Exception
    {
        public yyLibraryException (string message): base (message)
        {
        }

        public yyLibraryException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
