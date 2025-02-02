﻿using System.Text.Json;

namespace yyLib
{
    public static class yyGptChatResponseParser
    {
        public static yyGptChatResponse Parse (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException ($"'{nameof (str)}' is invalid: {str.GetVisibleString ()}");

            var xResponse = JsonSerializer.Deserialize <yyGptChatResponse> (str, yyJson.DefaultDeserializationOptions);

            if (xResponse == null)
                throw new yyFormatException ($"Failed to deserialize JSON: {str.GetVisibleString ()}");

            if (xResponse.Error != null)
                return xResponse;

            if (xResponse.Choices == null)
                throw new yyFormatException ($"The 'choices' property is missing: {str.GetVisibleString ()}");

            if (xResponse.Choices.Any (x =>
            {
                if (x.Message == null)
                    return true;

                if (string.IsNullOrWhiteSpace (x.Message.Content as string)) // Content is a string.
                    return true;

                return false;
            }))
                throw new yyFormatException ($"The 'choices' property is invalid: {str.GetVisibleString ()}");

            return xResponse;
        }

        /// <summary>
        /// Returns yyGptChatResponse.Empty when "data: [DONE]" is detected.
        /// </summary>
        public static yyGptChatResponse ParseChunk (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException ($"'{nameof (str)}' is invalid: {str.GetVisibleString ()}");

            if (str.StartsWith ("data: {", StringComparison.OrdinalIgnoreCase))
            {
                string xJsonString = str.Substring ("data: ".Length);

                var xResponse = JsonSerializer.Deserialize <yyGptChatResponse> (xJsonString, yyJson.DefaultDeserializationOptions);

                if (xResponse == null)
                    throw new yyFormatException ($"Failed to deserialize JSON: {xJsonString.GetVisibleString ()}");

                if (xResponse.Error != null)
                    return xResponse;

                if (xResponse.Choices == null)
                    throw new yyFormatException ($"The 'choices' property is missing: {xJsonString.GetVisibleString ()}");

                if (xResponse.Choices.Any () == false)
                    throw new yyFormatException ($"The 'choices' property is empty: {xJsonString.GetVisibleString ()}");

                if (xResponse.Choices.Any (x =>
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
                    throw new yyFormatException ($"The 'choices' property is invalid: {xJsonString.GetVisibleString ()}");

                return xResponse;
            }

            if (str.Equals ("data: [DONE]", StringComparison.OrdinalIgnoreCase))
                return yyGptChatResponse.Empty;

            throw new yyFormatException ($"Failed to parse chunk: {str.GetVisibleString ()}");
        }
    }
}
