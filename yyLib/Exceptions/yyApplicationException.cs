namespace yyLib
{
    public class yyApplicationException: ApplicationException
    {
        public yyApplicationException (string message): base (message)
        {
        }

        public yyApplicationException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
