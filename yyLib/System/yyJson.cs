using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace yyLib
{
    public static class yyJson
    {
        public static JsonSerializerOptions DefaultSerializationOptions { get; } = new ()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Needed for CJK characters.
            WriteIndented = true
        };

        public static JsonSerializerOptions DefaultDeserializationOptions { get; } = new ()
        {
            // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Doesnt affect anything here.
            // PropertyNameCaseInsensitive = true // Can be slower. There are more serious formatting issues than case sensitivity.
        };
    }
}
