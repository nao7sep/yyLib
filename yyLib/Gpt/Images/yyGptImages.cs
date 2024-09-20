namespace yyLib
{
    public static class yyGptImages
    {
        // yyGptLib has been merged into yyLib.
        // Example usage: https://github.com/nao7sep/yyGptLib/blob/main/yyGptLibConsole/Tester3.cs

        public static string DefaultEndpoint { get; } = yyUserSecrets.Default.OpenAi?.ImagesEndpoint.WhiteSpaceToNull () ?? "https://api.openai.com/v1/images/generations";
    }
}
