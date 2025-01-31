using System.Text.Json;
using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatToolJsonConverter: JsonConverter <object?>
    {
        public override void Write (Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (value is string xStr)
                writer.WriteStringValue (xStr);
            else if (value is yyGptChatTool xTool)
                JsonSerializer.Serialize (writer, xTool, options);
            else if (value is null)
                writer.WriteNullValue ();
            else throw new yyInvalidDataException ("Invalid type for 'tool'.");
        }

        public override object? Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString ();
            else if (reader.TokenType == JsonTokenType.StartObject)
                return JsonSerializer.Deserialize <yyGptChatTool> (ref reader, options);
            else if (reader.TokenType == JsonTokenType.Null)
                return null;
            else throw new yyInvalidDataException ("Invalid type for 'tool'.");
        }
    }
}
