using System.Text.Json;

namespace yyLib
{
    // todo: Could be a static class.
    public class yyUserSecretsParser
    {
        public JsonSerializerOptions DeserializationOptions { get; private set; }

        public yyUserSecretsParser (JsonSerializerOptions? deserializationOptions = null) =>
            DeserializationOptions = deserializationOptions ?? yyJson.DefaultDeserializationOptions;

        public yyUserSecretsModel Parse (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException (yyMessage.Create ($"'{nameof (str)}' is invalid."));
            // todo: Display the content.

            var xResponse = (yyUserSecretsModel?) JsonSerializer.Deserialize (str, typeof (yyUserSecretsModel), DeserializationOptions);

            if (xResponse == null)
                throw new yyFormatException (yyMessage.Create ("Failed to deserialize JSON."));

            return xResponse;
        }
    }
}
