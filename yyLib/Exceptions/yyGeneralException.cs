namespace yyLib
{
    public class yyGeneralException: Exception
    {
        public yyGeneralException (string message): base (message)
        {
        }

        public yyGeneralException (string message, Exception innerException): base (message, innerException)
        {
        }
    }
}
