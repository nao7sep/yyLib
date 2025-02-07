namespace yyLib
{
    public static class yyGptImagesValidator
    {
        // Each response contains multiple images.
        public static void ValidateImagesResponse (yyGptImagesResponse response, string responseJsonString)
        {
            if (response.Error != null)
                return;

            // https://platform.openai.com/docs/api-reference/images/object

            if (response.Data == null)
                throw new yyFormatException ($"The 'data' property is missing: {responseJsonString.GetVisibleString ()}");

            if (response.Data.Any () == false)
                throw new yyFormatException ($"The 'data' property is empty: {responseJsonString.GetVisibleString ()}");

            if (response.Data.Any (x =>
            {
                if (x.B64Json == null && x.Url == null)
                    return true;

                // DALL-E 2 model doesnt return revised prompts.
                // if (string.IsNullOrWhiteSpace (x.RevisedPrompt))
                //     return true;

                return false;
            }))
                throw new yyFormatException ($"The 'data' property is invalid: {responseJsonString.GetVisibleString ()}");
        }
    }
}
