namespace yyLib
{
    public class yyOperationException: yyException
    {
        public yyOperationException (string message) : base (message)
        {
        }

        public yyOperationException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
