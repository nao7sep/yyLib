namespace yyLib
{
    public class yyArgumentException: yyLibraryException
    {
        public yyArgumentException (string message): base (message)
        {
        }

        public yyArgumentException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
