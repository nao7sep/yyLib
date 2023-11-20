namespace yyLib
{
    internal class yyNotSupportedException: yyLibraryException
    {
        public yyNotSupportedException (yyMessage message): base (message)
        {
        }

        public yyNotSupportedException (yyMessage message, Exception innerException): base (message, innerException)
        {
        }
    }
}
