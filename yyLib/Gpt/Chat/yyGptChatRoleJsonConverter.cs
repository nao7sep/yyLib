using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatRoleJsonConverter: JsonConverter <yyGptChatRole>
    {
        // Suppresses the warning about normalizing strings to uppercase instead of lowercase (CA1308).
        [SuppressMessage ("Globalization", "CA1308")]
        public override void Write (Utf8JsonWriter writer, yyGptChatRole value, JsonSerializerOptions options) =>
            writer.WriteStringValue (yyConverter.EnumToString (value).ToLowerInvariant ());

        public override yyGptChatRole Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            yyConverter.StringToEnum <yyGptChatRole> (reader.GetString ()!, ignoreCase: true);
    }
}
