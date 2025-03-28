namespace yyLib
{
    public class yyDataException: yyException
    {
        public yyDataException (string message) : base (message)
        {
        }

        public yyDataException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
