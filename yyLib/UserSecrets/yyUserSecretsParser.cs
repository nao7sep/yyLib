using System.Text.Json.Serialization;
using System.Text.Json;

namespace yyLib
{
    public class yyUserSecretsParser
    {
        public JsonSerializerOptions JsonSerializerOptions { get; private set; }

        public yyUserSecretsParser () => JsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        public yyUserSecretsModel Parse (string? str)
        {
            if (string.IsNullOrWhiteSpace (str))
                throw new yyArgumentException ($"'{nameof (str)}' is invalid.");

            var xResponse = (yyUserSecretsModel?) JsonSerializer.Deserialize (str, typeof (yyUserSecretsModel), JsonSerializerOptions);

            if (xResponse == null)
                throw new yyFormatException ("Failed to deserialize JSON.");

            return xResponse;
        }
    }
}
