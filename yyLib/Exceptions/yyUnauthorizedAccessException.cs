namespace yyLib
{
    public class yyUnauthorizedAccessException: yyException
    {
        public yyUnauthorizedAccessException (string message) : base (message)
        {
        }

        public yyUnauthorizedAccessException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
