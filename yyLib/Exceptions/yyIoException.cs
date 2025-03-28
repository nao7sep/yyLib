namespace yyLib
{
    public class yyIoException: yyException
    {
        public yyIoException (string message) : base (message)
        {
        }

        public yyIoException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
