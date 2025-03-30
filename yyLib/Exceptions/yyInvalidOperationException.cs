namespace yyLib
{
    public class yyInvalidOperationException: yyException
    {
        public yyInvalidOperationException (string message): base (message)
        {
        }

        public yyInvalidOperationException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
