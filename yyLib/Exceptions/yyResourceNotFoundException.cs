namespace yyLib
{
    public class yyResourceNotFoundException: yyException
    {
        public yyResourceNotFoundException (string message) : base (message)
        {
        }

        public yyResourceNotFoundException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
