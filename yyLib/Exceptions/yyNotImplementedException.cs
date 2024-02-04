namespace yyLib
{
    public class yyNotImplementedException: yyLibraryException
    {
        public yyNotImplementedException (string message): base (message)
        {
        }

        public yyNotImplementedException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
