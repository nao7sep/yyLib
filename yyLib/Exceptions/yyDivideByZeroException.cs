namespace yyLib
{
    public class yyDivideByZeroException: yyException
    {
        public yyDivideByZeroException (string message) : base (message)
        {
        }

        public yyDivideByZeroException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
