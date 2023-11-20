namespace yyLib
{
    public class yyInvalidDataException: yyLibraryException
    {
        public yyInvalidDataException (yyMessage message): base (message)
        {
        }

        public yyInvalidDataException (yyMessage message, Exception innerException): base (message, innerException)
        {
        }
    }
}
