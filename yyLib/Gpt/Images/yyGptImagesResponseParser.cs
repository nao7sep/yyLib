using System.Text.Json;

namespace yyLib
{
    public static class yyGptImagesResponseParser
    {
        public static yyGptImagesResponse Parse (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException ($"'{nameof (str)}' is invalid: {str.GetVisibleString ()}");

            var xResponse = JsonSerializer.Deserialize <yyGptImagesResponse> (str, yyJson.DefaultDeserializationOptions);

            if (xResponse == null)
                throw new yyFormatException ($"Failed to deserialize JSON: {str.GetVisibleString ()}");

            // Validation is not a parser's responsibility.
            // Use yyGptImagesValidator.

            return xResponse;
        }
    }
}
