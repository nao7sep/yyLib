namespace yyLib
{
    public class yyNotImplementedException: yyException
    {
        public yyNotImplementedException (string message) : base (message)
        {
        }

        public yyNotImplementedException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
