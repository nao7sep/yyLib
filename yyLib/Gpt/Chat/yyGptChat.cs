using yyLib;

namespace yyGptLib
{
    public static class yyGptChat
    {
        public static string DefaultEndpoint { get; } = yyUserSecrets.Default.OpenAi?.ChatEndpoint.WhiteSpaceToNull () ?? "https://api.openai.com/v1/chat/completions";

        // https://platform.openai.com/docs/models/gpt-4-turbo-and-gpt-4
        public static string DefaultModel { get; } = yyUserSecrets.Default.OpenAi?.ChatModel.WhiteSpaceToNull () ?? "gpt-4-turbo";
    }
}
