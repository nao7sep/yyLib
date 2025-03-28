namespace yyLib
{
    public class yyTimeoutException: yyException
    {
        public yyTimeoutException (string message) : base (message)
        {
        }

        public yyTimeoutException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
