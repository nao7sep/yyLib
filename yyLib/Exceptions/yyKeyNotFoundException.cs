namespace yyLib
{
    public class yyKeyNotFoundException: yyException
    {
        public yyKeyNotFoundException (string message) : base (message)
        {
        }

        public yyKeyNotFoundException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
