using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyGptImagesConnectionInfo: yyGptConnectionInfo
    {
        // -----------------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------------

        public static readonly string DefaultEndpoint = "https://api.openai.com/v1/images/generations";

        public static yyGptImagesConnectionInfo ConvertFromBase (yyGptConnectionInfo gptConnectionInfo)
        {
            return new yyGptImagesConnectionInfo
            {
                ApiKey = gptConnectionInfo.ApiKey,
                Organization = gptConnectionInfo.Organization,
                Project = gptConnectionInfo.Project,
                Endpoint = gptConnectionInfo.Endpoint,
                Timeout = gptConnectionInfo.Timeout
            };
        }

        // Suppresses the warning about catching a general exception (CA1031).
        [SuppressMessage ("Design", "CA1031")]
        private static yyGptImagesConnectionInfo _CreateDefault ()
        {
            var xGptImagesConnectionSection = yyAppSettings.Config.GetSection ("gpt_images_connection");

            if (xGptImagesConnectionSection.Exists () &&
                xGptImagesConnectionSection.GetChildren ().Any () &&
                xGptImagesConnectionSection.Get <yyGptImagesConnectionInfo> () is { } xGptImagesConnectionInfo)
                    return xGptImagesConnectionInfo;

            if (yyUserSecrets.Default.GptImagesConnection != null)
                return yyUserSecrets.Default.GptImagesConnection;

            try
            {
                var xGptConnectionInfo = yyGptConnectionInfo.Default;
                return ConvertFromBase (xGptConnectionInfo);
            }

            catch
            {
                // The exception from accessing the Default property is disregarded because this is a fallback mechanism.
                // The purpose here is to try our luck and proceed only if it succeeds.
            }

            throw new yyInvalidDataException ("No GPT images connection info found.");
        }

        private static readonly Lazy <yyGptImagesConnectionInfo> _default = new (_CreateDefault ());

        public static new yyGptImagesConnectionInfo Default => _default.Value;
    }
}
