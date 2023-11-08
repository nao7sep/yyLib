namespace yyLib
{
    public static class yyUserSecretsDefaultValues
    {
        public static string FileName { get; } = ".yyUserSecrets";

        private static readonly Lazy <string []> _filePaths = new (() => new []
        {
            // By default, the following file paths are tried in order.
            // If any file exists, yyUserSecretsLoader.Load attempts to deserialize it and tries no further.

            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), FileName),
            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), FileName),
            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), FileName),
            Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData), FileName),
            Path.Join (Path.GetDirectoryName (yyLibraryAssembly.Location), FileName)
        });

        public static string [] FilePaths => _filePaths.Value;
    }
}
