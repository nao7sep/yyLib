using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public class yyGptChatConnectionInfo: yyGptConnectionInfo
    {
        // -----------------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------------

        public static readonly string DefaultEndpoint = "https://api.openai.com/v1/chat/completions";

        private yyGptChatConnectionInfo _CopyMissingValues ()
        {
            _CopyMissingValues (this, yyGptConnectionInfo.Default);
            return this;
        }

        private static yyGptChatConnectionInfo _CreateDefault ()
        {
            var xGptChatConnectionSection = yyAppSettings.Config.GetSection ("gpt_chat_connection");

            if (xGptChatConnectionSection.Exists () &&
                xGptChatConnectionSection.GetChildren ().Any () &&
                xGptChatConnectionSection.Get <yyGptChatConnectionInfo> () is { } xGptChatConnectionInfo)
                    return xGptChatConnectionInfo._CopyMissingValues ();

            if (yyUserSecrets.Default.GptChatConnection != null)
                return yyUserSecrets.Default.GptChatConnection._CopyMissingValues ();

            return new yyGptChatConnectionInfo ()
            {
                Endpoint = DefaultEndpoint,
                Timeout = DefaultTimeout
            }.
            _CopyMissingValues ();
        }

        private static readonly Lazy <yyGptChatConnectionInfo> _default = new (() => _CreateDefault ());

        public static new yyGptChatConnectionInfo Default => _default.Value;
    }
}
