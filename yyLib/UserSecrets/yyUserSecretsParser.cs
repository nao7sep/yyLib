using System.Text.Json;

namespace yyLib
{
    public static class yyUserSecretsParser
    {
        public static yyUserSecrets Parse (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException ($"'{nameof (str)}' is invalid: {str.GetVisibleString ()}");

            var xResponse = (yyUserSecrets?) JsonSerializer.Deserialize (str, typeof (yyUserSecrets), yyJson.DefaultDeserializationOptions);

            if (xResponse == null)
                throw new yyFormatException ($"Failed to deserialize JSON: {str.GetVisibleString ()}");

            return xResponse;
        }
    }
}
