using System.Text.Json;
using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatContentJsonConverter: JsonConverter <object?>
    {
        public override void Write (Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (value is string xStr)
                writer.WriteStringValue (xStr);
            else if (value is IList <yyGptChatContentPart> xParts)
                JsonSerializer.Serialize (writer, xParts, options);
            else if (value is null)
                writer.WriteNullValue ();
            else throw new yyInvalidDataException ("Invalid type for 'content'.");
        }

        public override object? Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString ();
            else if (reader.TokenType == JsonTokenType.StartArray)
                return JsonSerializer.Deserialize <IList <yyGptChatContentPart>> (ref reader, options);
            else if (reader.TokenType == JsonTokenType.Null)
                return null;
            else throw new yyInvalidDataException ("Invalid type for 'content'.");
        }
    }
}
