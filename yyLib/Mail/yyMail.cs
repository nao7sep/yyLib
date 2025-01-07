namespace yyLib
{
    public static class yyMail
    {
        /// <summary>
        /// Default connection is available only when one is specified in the user secrets.
        /// </summary>
        public static yyMailConnectionInfo? DefaultConnection => yyUserSecrets.Default.MailConnection;
    }
}
