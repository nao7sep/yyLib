namespace yyLib
{
    public class yyNotImplementedException: yyLibraryException
    {
        public yyNotImplementedException (yyMessage message): base (message)
        {
        }

        public yyNotImplementedException (yyMessage message, Exception innerException): base (message, innerException)
        {
        }
    }
}
