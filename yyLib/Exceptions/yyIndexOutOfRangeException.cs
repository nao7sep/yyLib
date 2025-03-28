namespace yyLib
{
    public class yyIndexOutOfRangeException: yyException
    {
        public yyIndexOutOfRangeException (string message) : base (message)
        {
        }

        public yyIndexOutOfRangeException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
