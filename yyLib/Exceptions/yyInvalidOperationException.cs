namespace yyLib
{
    internal class yyInvalidOperationException: yyGeneralException
    {
        public yyInvalidOperationException (string message): base (message)
        {
        }

        public yyInvalidOperationException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
