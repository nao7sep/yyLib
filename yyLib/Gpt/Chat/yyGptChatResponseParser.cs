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

            return xResponse;
        }

        /// <summary>
        /// Returns (null, null) when "data: [DONE]" is detected.
        /// </summary>
        public static (yyGptChatResponse? Response, string? JsonString) ParseChunk (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException ($"'{nameof (str)}' is invalid: {str.GetVisibleString ()}");

            if (str.StartsWith ("data: {", StringComparison.OrdinalIgnoreCase))
            {
                string xJsonString = str.Substring ("data: ".Length);

                var xResponse = JsonSerializer.Deserialize <yyGptChatResponse> (xJsonString, yyJson.DefaultDeserializationOptions);

                if (xResponse == null)
                    throw new yyFormatException ($"Failed to deserialize JSON: {xJsonString.GetVisibleString ()}");

                return (xResponse, xJsonString);
            }

            if (str.Equals ("data: [DONE]", StringComparison.OrdinalIgnoreCase))
                return (null, null);

            throw new yyFormatException ($"Failed to parse chunk: {str.GetVisibleString ()}");
        }
    }
}
