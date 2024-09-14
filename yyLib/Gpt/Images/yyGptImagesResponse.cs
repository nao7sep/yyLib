using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptImagesResponse
    {
        // This model class should be able to deserialize something like the example on this page:
        // https://platform.openai.com/docs/api-reference/images/create
        // I havent found any official documentation regarding this JSON string representation.

        // The properties are sorted in the order of the example in the API reference.

        // Just like yyGptChatResponse, "error" is included.
        // The developer shouldnt have to deal with 2 different model classes.

        [JsonPropertyName ("created")]
        public int? Created { get; set; }

        [JsonPropertyName ("data")]
        public IList <yyGptImagesResponseImage>? Data { get; set; }

        [JsonPropertyName ("error")]
        public yyGptResponseError? Error { get; set; }
    }
}
