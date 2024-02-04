namespace yyLib
{
    internal class yyNotSupportedException: yyLibraryException
    {
        public yyNotSupportedException (string message): base (message)
        {
        }

        public yyNotSupportedException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
