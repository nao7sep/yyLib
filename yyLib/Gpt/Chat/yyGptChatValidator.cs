namespace yyLib
{
    public static class yyGptChatValidator
    {
        // Each response contains multiple messages.
        public static void ValidateMessagesResponse (yyGptChatResponse response, string responseJsonString)
        {
            if (response.Error != null)
                return;

            // https://platform.openai.com/docs/api-reference/chat/object

            if (response.Choices == null)
                throw new yyFormatException ($"The 'choices' property is missing: {responseJsonString.GetVisibleString ()}");

            if (response.Choices.Any () == false)
                throw new yyFormatException ($"The 'choices' property is empty: {responseJsonString.GetVisibleString ()}");

            if (response.Choices.Any (x =>
            {
                if (x.Message == null)
                    return true;

                if (string.IsNullOrWhiteSpace (x.Message.Content as string)) // Content is a string.
                    return true;

                return false;
            }))
                throw new yyFormatException ($"The 'choices' property is invalid: {responseJsonString.GetVisibleString ()}");
        }

        // Each response contains a chunk of a single message.
        public static void ValidateMessageChunkResponse (yyGptChatResponse response, string responseJsonString)
        {
            if (response.Error != null)
                return;

            // https://platform.openai.com/docs/api-reference/chat/streaming

            if (response.Choices == null)
                throw new yyFormatException ($"The 'choices' property is missing: {responseJsonString.GetVisibleString ()}");

            if (response.Choices.Any () == false)
                throw new yyFormatException ($"The 'choices' property is empty: {responseJsonString.GetVisibleString ()}");

            if (response.Choices.Any (x =>
            {
                if (x.Index == null)
                    return true;

                if (x.Delta == null)
                    return true;

                // The "Content" property is not checked because the following (partial) responses have been observed:
                // "choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}]
                // "choices":[{"index":0,"delta":{},"logprobs":null,"finish_reason":"stop"}]
                // "Content" can be empty or null.

                return false;
            }))
                throw new yyFormatException ($"The 'choices' property is invalid: {responseJsonString.GetVisibleString ()}");
        }
    }
}
