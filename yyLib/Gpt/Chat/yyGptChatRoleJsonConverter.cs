using System.Text.Json;
using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatRoleJsonConverter: JsonConverter <yyGptChatRole>
    {
        public override void Write (Utf8JsonWriter writer, yyGptChatRole value, JsonSerializerOptions options) =>
            writer.WriteStringValue (yyConvertor.EnumToString (value).ToLowerInvariant ());

        public override yyGptChatRole Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            yyConvertor.StringToEnum <yyGptChatRole> (reader.GetString ()!, ignoreCase: true);
    }
}
