namespace yyLib
{
    public class yyLibraryException: Exception
    {
        public yyLibraryException (yyMessage message): base (message.Content)
        {
        }

        public yyLibraryException (yyMessage message, Exception innerException): base (message.Content, innerException)
        {
        }
    }
}
