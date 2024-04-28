namespace yyLib
{
    public class yyNotSupportedException: yyGeneralException
    {
        public yyNotSupportedException (string message): base (message)
        {
        }

        public yyNotSupportedException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
