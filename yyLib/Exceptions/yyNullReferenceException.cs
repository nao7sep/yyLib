namespace yyLib
{
    public class yyNullReferenceException: yyException
    {
        public yyNullReferenceException (string message) : base (message)
        {
        }

        public yyNullReferenceException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
