namespace yyLib
{
    public static class yyGptChatRequestHelper
    {
        public static void AddMessage (this yyGptChatRequest request, yyGptChatMessageRole role, string content, string? name = null)
        {
            request.Messages ??= new List <yyGptChatMessage> ();

            request.Messages.Add (new yyGptChatMessage
            {
                Role = role,
                Content = content,
                Name = name
            });
        }

        public static void AddSystemMessage (this yyGptChatRequest request, string content, string? name = null) =>
            AddMessage (request, yyGptChatMessageRole.System, content, name);

        public static void AddUserMessage (this yyGptChatRequest request, string content, string? name = null) =>
            AddMessage (request, yyGptChatMessageRole.User, content, name);

        public static void AddAssistantMessage (this yyGptChatRequest request, string content, string? name = null) =>
            AddMessage (request, yyGptChatMessageRole.Assistant, content, name);
    }
}
