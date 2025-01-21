using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyGptChatConnectionInfo: yyGptConnectionInfo
    {
        // -----------------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------------

        public static readonly string DefaultEndpoint = "https://api.openai.com/v1/chat/completions";

        public static yyGptChatConnectionInfo ConvertFromBase (yyGptConnectionInfo gptConnectionInfo)
        {
            return new yyGptChatConnectionInfo
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
        private static yyGptChatConnectionInfo _CreateDefault ()
        {
            var xGptChatConnectionSection = yyAppSettings.Config.GetSection ("gpt_chat_connection");

            if (xGptChatConnectionSection.Exists () &&
                xGptChatConnectionSection.GetChildren ().Any () &&
                xGptChatConnectionSection.Get <yyGptChatConnectionInfo> () is { } xGptChatConnectionInfo)
                    return xGptChatConnectionInfo;

            if (yyUserSecrets.Default.GptChatConnection != null)
                return yyUserSecrets.Default.GptChatConnection;

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

            throw new yyInvalidDataException ("No GPT chat connection info found.");
        }

        private static readonly Lazy <yyGptChatConnectionInfo> _default = new (_CreateDefault ());

        public static new yyGptChatConnectionInfo Default => _default.Value;
    }
}
