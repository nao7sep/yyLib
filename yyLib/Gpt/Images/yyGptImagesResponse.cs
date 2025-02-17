﻿using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptImagesResponse
    {
        // This model class should be able to deserialize something like the example on this page:
        // https://platform.openai.com/docs/api-reference/images/create
        // I havent found any official documentation regarding this JSON string representation.

        // Just like yyGptChatResponse, "error" is included.
        // The developer shouldnt have to deal with 2 different model classes.

        [JsonPropertyName ("created")]
        public int? Created { get; set; }

        [JsonPropertyName ("data")]
        public IList <yyGptImagesImage>? Data { get; set; }

        [JsonPropertyName ("error")]
        public yyGptError? Error { get; set; }
    }
}
