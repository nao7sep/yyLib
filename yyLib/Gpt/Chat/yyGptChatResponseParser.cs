using System.Text.Json;

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

            // Validation is not a parser's responsibility.
            // Use yyGptChatValidator.

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

                // Validation is not a parser's responsibility.
                // Use yyGptChatValidator.

                return xResponse;
            }

            if (str.Equals ("data: [DONE]", StringComparison.OrdinalIgnoreCase))
                return yyGptChatResponse.Empty;

            throw new yyFormatException ($"Failed to parse chunk: {str.GetVisibleString ()}");
        }
    }
}
