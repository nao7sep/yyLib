namespace yyLib
{
    internal class yyInvalidOperationException: yyLibraryException
    {
        public yyInvalidOperationException (string message): base (message)
        {
        }

        public yyInvalidOperationException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
