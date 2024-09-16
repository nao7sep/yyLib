namespace yyLib
{
    public class yyNotSupportedException: yyException
    {
        public yyNotSupportedException (string message): base (message)
        {
        }

        public yyNotSupportedException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
