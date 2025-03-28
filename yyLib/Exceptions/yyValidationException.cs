namespace yyLib
{
    public class yyValidationException: yyException
    {
        public yyValidationException (string message) : base (message)
        {
        }

        public yyValidationException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
