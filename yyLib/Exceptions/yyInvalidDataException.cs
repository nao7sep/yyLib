namespace yyLib
{
    public class yyInvalidDataException: yyLibraryException
    {
        public yyInvalidDataException (string message): base (message)
        {
        }

        public yyInvalidDataException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
