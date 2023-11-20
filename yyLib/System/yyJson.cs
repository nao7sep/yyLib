using System.Text.Json.Serialization;
using System.Text.Json;

namespace yyLib
{
    public static class yyJson
    {
        public static JsonSerializerOptions DefaultSerializationOptions { get; } = new ()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        public static JsonSerializerOptions DefaultDeserializationOptions { get; } = new ()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
    }
}
