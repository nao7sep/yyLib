namespace yyLib
{
    public class yyInvalidDataException: yyException
    {
        public yyInvalidDataException (string message): base (message)
        {
        }

        public yyInvalidDataException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
