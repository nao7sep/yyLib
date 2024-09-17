using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyUserSecrets
    {
        [JsonPropertyName ("openai")]
        public yyUserSecretsOpenAi? OpenAi { get; set; }

        // -----------------------------------------------------------------------------
        //     Static Members
        // -----------------------------------------------------------------------------

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

        // In yyLib, User Secrets are defined statically.
        // These are NOT user settings; they are more like special values that are guaranteed to be supported by the library.
        // Therefore, we wont casually add new values and, once added, everything must remain backward compatible.
    }
}
