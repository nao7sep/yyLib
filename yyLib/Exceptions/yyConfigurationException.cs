namespace yyLib
{
    public class yyConfigurationException: yyException
    {
        public yyConfigurationException (string message) : base (message)
        {
        }

        public yyConfigurationException (string message, Exception innerException) : base (message, innerException)
        {
        }
    }
}
