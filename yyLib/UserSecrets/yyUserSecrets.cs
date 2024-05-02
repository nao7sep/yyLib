using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyUserSecrets
    {
        public static string DefaultFileName { get; } = ".yyUserSecrets.json";

        private static readonly Lazy <string []> _defaultFilePaths = new (() => new []
        {
            // By default, the following file paths are tried in order.
            // If any file exists, yyUserSecretsLoader.Load attempts to deserialize it and tries no more.

            Path.Join (yySpecialDirectories.UserProfile, DefaultFileName),
            Path.Join (yySpecialDirectories.AppData, DefaultFileName),
            Path.Join (yySpecialDirectories.LocalAppData, DefaultFileName),
            Path.Join (yySpecialDirectories.CommonAppData, DefaultFileName),
            yyAppDirectory.MapPath (DefaultFileName)
        });

        public static string [] DefaultFilePaths => _defaultFilePaths.Value;

        // The following code used to be a separate model class.
        // If the model class contains Default, it can and should have DefaultFileName and other things as well.

        private static readonly Lazy <yyUserSecrets> _default = new (() => yyUserSecretsLoader.Load (DefaultFilePaths));

        /// <summary>
        /// NOT thread-safe.
        /// </summary>
        public static yyUserSecrets Default => _default.Value;

        [JsonPropertyName ("openai")]
        public yyUserSecretsOpenAi? OpenAi { get; set; }
    }
}
