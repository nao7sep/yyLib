namespace yyLib
{
    public static class yyGptChat
    {
        // https://platform.openai.com/docs/api-reference/chat

        // https://platform.openai.com/docs/models/gpt-4o
        public static readonly string DefaultModel = "gpt-4o";

        // The 'reasoning_effort' parameter is applicable exclusively to o1 models.
        // It accepts the following string values: 'low', 'medium', and 'high'.
        // This parameter constrains the model's reasoning effort, with 'medium' as the default setting.
        // Lowering the reasoning effort can lead to faster responses and reduce the number of tokens used for reasoning in a response.
        public static readonly string DefaultReasoningEffort = "medium";
    }
}
