namespace yyLib
{
    public static class yyUserSecrets
    {
        public static string DefaultFileName { get; } = ".yyUserSecrets.json";

        private static readonly Lazy <string []> _defaultFilePaths = new (() => new []
        {
            // By default, the following file paths are tried in order.
            // If any file exists, yyUserSecretsLoader.Load attempts to deserialize it and tries no more.

            Path.Join (yySpecialDirectories.UserProfile, DefaultFileName),
            Path.Join (yySpecialDirectories.ApplicationData, DefaultFileName),
            Path.Join (yySpecialDirectories.LocalApplicationData, DefaultFileName),
            Path.Join (yySpecialDirectories.CommonApplicationData, DefaultFileName),
            yyApplicationDirectory.MapPath (DefaultFileName)
        });

        public static string [] DefaultFilePaths => _defaultFilePaths.Value;
    }
}
