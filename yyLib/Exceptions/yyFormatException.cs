namespace yyLib
{
    public class yyFormatException: yyLibraryException
    {
        public yyFormatException (yyMessage message): base (message)
        {
        }

        public yyFormatException (yyMessage message, Exception innerException): base (message, innerException)
        {
        }
    }
}
