namespace yyLib
{
    public class yyNotImplementedException: yyGeneralException
    {
        public yyNotImplementedException (string message): base (message)
        {
        }

        public yyNotImplementedException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
