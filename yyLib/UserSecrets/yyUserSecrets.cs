﻿using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyUserSecrets
    {
        [JsonPropertyName ("gpt_connection")]
        public yyGptConnectionInfo? GptConnection { get; set; }

        [JsonPropertyName ("gpt_chat_connection")]
        public yyGptChatConnectionInfo? GptChatConnection { get; set; }

        [JsonPropertyName ("gpt_images_connection")]
        public yyGptImagesConnectionInfo? GptImagesConnection { get; set; }

        [JsonPropertyName ("logger")]
        public yyLogger? Logger { get; set; }

        [JsonPropertyName ("mail_connection")]
        public yyMailConnectionInfo? MailConnection { get; set; }

        // -----------------------------------------------------------------------------
        // Static Members
        // -----------------------------------------------------------------------------

        public static string DefaultFileName { get; } = ".yyUserSecrets.json";

        private static readonly Lazy <string []> _defaultFilePaths = new (() =>
        [
            // By default, the following file paths are tried in order.
            // If any file exists, yyUserSecretsLoader.Load attempts to deserialize it and tries no more.

            yyPath.Join (yySpecialDirectories.UserProfile, DefaultFileName),
            yyPath.Join (yySpecialDirectories.AppData, DefaultFileName),
            yyPath.Join (yySpecialDirectories.LocalAppData, DefaultFileName),
            yyPath.Join (yySpecialDirectories.CommonAppData, DefaultFileName),
            yyAppDirectory.MapPath (DefaultFileName)
        ]);

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
