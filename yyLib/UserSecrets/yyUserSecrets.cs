namespace yyLib
{
    public static class yyUserSecrets
    {
        public static string DefaultFileName { get; } = ".yyUserSecrets";

        private static readonly Lazy <string []> _defaultFilePaths = new (() => new []
        {
            // By default, the following file paths are tried in order.
            // If any file exists, yyUserSecretsLoader.Load attempts to deserialize it and tries no more.

            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), DefaultFileName),
            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), DefaultFileName),
            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), DefaultFileName),
            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData), DefaultFileName),
            yyApplicationDirectory.MapPath (DefaultFileName)
        });

        public static string [] DefaultFilePaths => _defaultFilePaths.Value;
    }
}
