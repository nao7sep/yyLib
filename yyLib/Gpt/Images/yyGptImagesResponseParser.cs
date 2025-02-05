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

            if (xResponse.Error != null)
                return xResponse;

            if (xResponse.Data == null)
                throw new yyFormatException ($"The 'data' property is missing: {str.GetVisibleString ()}");

            if (xResponse.Data.Any (x =>
            {
                if (x.B64Json == null && x.Url == null)
                    return true;

                // DALL-E 2 model doesnt return revised prompts.
                // if (string.IsNullOrWhiteSpace (x.RevisedPrompt))
                //     return true;

                return false;
            }))
                throw new yyFormatException ($"The 'data' property is invalid: {str.GetVisibleString ()}");

            return xResponse;
        }
    }
}
