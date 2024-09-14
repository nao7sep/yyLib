using System.Text.Json;
using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptChatMessageRoleJsonConverter: JsonConverter <yyGptChatMessageRole>
    {
        public override void Write (Utf8JsonWriter writer, yyGptChatMessageRole value, JsonSerializerOptions options) =>
            writer.WriteStringValue (value.ToString ());

        public override yyGptChatMessageRole Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            yyGptChatMessageRole.Parse (reader.GetString ());
    }
}
