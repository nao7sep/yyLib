namespace yyLib
{
    public class yyOverflowException: yyException
    {
        public yyOverflowException (string message) : base (message)
        {
        }

        public yyOverflowException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
