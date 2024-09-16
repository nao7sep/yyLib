namespace yyLib
{
    public class yyException: Exception
    {
        public yyException (string message): base (message)
        {
        }

        public yyException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
