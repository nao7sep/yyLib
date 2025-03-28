namespace yyLib
{
    public class yySecurityException: yyException
    {
        public yySecurityException (string message) : base (message)
        {
        }

        public yySecurityException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
