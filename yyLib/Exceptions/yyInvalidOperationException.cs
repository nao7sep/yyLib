namespace yyLib
{
    internal class yyInvalidOperationException: yyLibraryException
    {
        public yyInvalidOperationException (yyMessage message): base (message)
        {
        }

        public yyInvalidOperationException (yyMessage message, Exception innerException): base (message, innerException)
        {
        }
    }
}
