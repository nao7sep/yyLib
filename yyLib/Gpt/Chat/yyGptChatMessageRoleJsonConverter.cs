using System.Text.Json;
using System.Text.Json.Serialization;
using yyLib;

namespace yyGptLib
{
    public class yyGptChatMessageRoleJsonConverter: JsonConverter <yyGptChatMessageRole>
    {
        public override void Write (Utf8JsonWriter writer, yyGptChatMessageRole value, JsonSerializerOptions options) =>
            writer.WriteStringValue (yyConvertor.EnumToString (value).ToLowerInvariant ());

        public override yyGptChatMessageRole Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            yyConvertor.StringToEnum <yyGptChatMessageRole> (reader.GetString ()!, ignoreCase: true);
    }
}
