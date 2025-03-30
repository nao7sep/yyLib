namespace yyLib
{
    public class yyUnexpectedNullException: yyException
    {
        public yyUnexpectedNullException (string message): base (message)
        {
        }

        public yyUnexpectedNullException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
