namespace yyLib
{
    public class yyNetworkException: yyException
    {
        public yyNetworkException (string message) : base (message)
        {
        }

        public yyNetworkException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
