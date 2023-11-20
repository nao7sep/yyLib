namespace yyLib
{
    public class yyApplicationException: ApplicationException
    {
        public yyApplicationException (yyMessage message): base (message.Content)
        {
        }

        public yyApplicationException (yyMessage message, Exception innerException): base (message.Content, innerException)
        {
        }
    }
}
