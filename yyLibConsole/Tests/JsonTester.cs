using System.Text.Json;
using yyLib;

namespace yyLibConsole
{
    internal static class JsonTester
    {
        /// <summary>
        /// Tests the parsing and display of the file "json_edge_cases.json".
        /// </summary>
        public static void Test1 ()
        {
            string xJsonFilePath = yyAppDirectory.MapPath ("json_edge_cases.json"),
                   xJsonString = File.ReadAllText (xJsonFilePath, yyEncoding.DefaultEncoding);

            using JsonDocument xDocument = JsonDocument.Parse (xJsonString);

            void _DisplayElement (string? key, JsonElement element, int indentLevel, bool isArrayElement)
            {
                string xFullIndent = new (' ', indentLevel * 4),
                       xKeyPart1 = string.IsNullOrEmpty (key) == false ? $" {JsonSerializer.Serialize (key)}" : string.Empty,
                       xKeyPart2 = isArrayElement ? string.Empty : $"{JsonSerializer.Serialize (key)}: ";

                /// A JSON document can have either an object (enclosed in `{}`) or an array (enclosed in `[]`) as its root element, as specified by the JSON standard (ECMA-404).
                /// An object consists of key-value pairs where keys are strings and values can be any valid JSON type, such as another object, array, string, number, boolean, or null.
                /// An array is an ordered list of values, which can also include any valid JSON type.
                /// Primitive types like strings, numbers, or booleans cannot serve as the root of a JSON document but can exist as values within objects or arrays.

                if (element.ValueKind == JsonValueKind.Object)
                {
                    if (element.Equals (xDocument.RootElement))
                        Console.WriteLine ($"{xFullIndent}Root Object:");
                    else Console.WriteLine ($"{xFullIndent}Object{xKeyPart1}:");

                    foreach (JsonProperty xProperty in element.EnumerateObject ())
                        _DisplayElement (xProperty.Name, xProperty.Value, indentLevel + 1, isArrayElement: false);
                }

                else if (element.ValueKind == JsonValueKind.Array)
                {
                    if (element.Equals (xDocument.RootElement))
                        Console.WriteLine ($"{xFullIndent}Root Array:");
                    else Console.WriteLine ($"{xFullIndent}Array{xKeyPart1}:");

                    foreach (JsonElement xElement in element.EnumerateArray ())
                        _DisplayElement (string.Empty, xElement, indentLevel + 1, isArrayElement: true);
                }

                else if (element.ValueKind == JsonValueKind.String)
                    Console.WriteLine ($"{xFullIndent}{xKeyPart2}{JsonSerializer.Serialize (element.GetString ())}");

                else if (element.ValueKind == JsonValueKind.Number)
                    Console.WriteLine ($"{xFullIndent}{xKeyPart2}{element.GetDouble ()}");

                else if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
                    Console.WriteLine ($"{xFullIndent}{xKeyPart2}{element.GetBoolean ()}");

                else if (element.ValueKind == JsonValueKind.Null)
                    Console.WriteLine ($"{xFullIndent}{xKeyPart2}Null");

                else throw new yyInvalidDataException ($"Unexpected JSON value kind: {element.ValueKind}");
            }

            _DisplayElement (null, xDocument.RootElement, 0, isArrayElement: xDocument.RootElement.ValueKind == JsonValueKind.Array);
        }
    }
}
