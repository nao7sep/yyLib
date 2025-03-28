namespace yyLib
{
    public class yyFormatException: yyException
    {
        public yyFormatException (string message) : base (message)
        {
        }

        public yyFormatException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
