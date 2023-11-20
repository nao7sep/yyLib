namespace yyLib
{
    public class yyObjectDisposedException: yyLibraryException
    {
        public yyObjectDisposedException (yyMessage message): base (message)
        {
        }

        public yyObjectDisposedException (yyMessage message, Exception innerException): base (message, innerException)
        {
        }
    }
}
