namespace yyLib
{
    public class yyObjectDisposedException: yyLibraryException
    {
        public yyObjectDisposedException (string message): base (message)
        {
        }

        public yyObjectDisposedException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
