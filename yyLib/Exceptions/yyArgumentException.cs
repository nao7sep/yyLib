namespace yyLib
{
    public class yyArgumentException: yyLibraryException
    {
        public yyArgumentException (yyMessage message): base (message)
        {
        }

        public yyArgumentException (yyMessage message, Exception innerException): base (message, innerException)
        {
        }
    }
}
