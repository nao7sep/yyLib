using System.Text.Json;

namespace yyLib
{
    public static class yyUserSecretsParser
    {
        public static yyUserSecretsModel Parse (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException (yyMessage.Create ($"'{nameof (str)}' is invalid: {str.GetVisibleString ()}"));

            var xResponse = (yyUserSecretsModel?) JsonSerializer.Deserialize (str, typeof (yyUserSecretsModel), yyJson.DefaultDeserializationOptions);

            if (xResponse == null)
                throw new yyFormatException (yyMessage.Create ($"Failed to deserialize JSON: {str.GetVisibleString ()}"));

            return xResponse;
        }
    }
}
