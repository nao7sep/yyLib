using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyGptImagesConnectionInfo: yyGptConnectionInfo
    {
        // -----------------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------------

        public static readonly string DefaultEndpoint = "https://api.openai.com/v1/images/generations";

        private yyGptImagesConnectionInfo _CopyMissingValues ()
        {
            _CopyMissingValues (this, yyGptConnectionInfo.Default);
            return this;
        }

        private static yyGptImagesConnectionInfo _CreateDefault ()
        {
            var xGptImagesConnectionSection = yyAppSettings.Config.GetSection ("gpt_images_connection");

            if (xGptImagesConnectionSection.Exists () &&
                xGptImagesConnectionSection.GetChildren ().Any () &&
                xGptImagesConnectionSection.Get <yyGptImagesConnectionInfo> () is { } xGptImagesConnectionInfo)
                    return xGptImagesConnectionInfo._CopyMissingValues ();

            if (yyUserSecrets.Default.GptImagesConnection != null)
                return yyUserSecrets.Default.GptImagesConnection._CopyMissingValues ();

            return new yyGptImagesConnectionInfo ()
            {
                Endpoint = DefaultEndpoint,
                Timeout = DefaultTimeout
            }.
            _CopyMissingValues ();
        }

        private static readonly Lazy <yyGptImagesConnectionInfo> _default = new (() => _CreateDefault ());

        public static new yyGptImagesConnectionInfo Default => _default.Value;
    }
}
