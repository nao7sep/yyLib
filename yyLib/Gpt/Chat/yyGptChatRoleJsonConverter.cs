using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatRoleJsonConverter: JsonConverter <yyGptChatRole?>
    {
        // Suppresses the warning about normalizing strings to uppercase instead of lowercase (CA1308).
        [SuppressMessage ("Globalization", "CA1308")]
        public override void Write (Utf8JsonWriter writer, yyGptChatRole? value, JsonSerializerOptions options)
        {
            if (value != null)
                writer.WriteStringValue (value.Value.ToString ().ToLowerInvariant ());
            else writer.WriteNullValue ();
        }

        public override yyGptChatRole? Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return yyConverter.StringToEnum <yyGptChatRole> (reader.GetString ()!, ignoreCase: true); // Not null.
            else if (reader.TokenType == JsonTokenType.Null)
                return null;
            else throw new yyInvalidDataException ("Invalid type for 'role'.");
        }
    }
}
