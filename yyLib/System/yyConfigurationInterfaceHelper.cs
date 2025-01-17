using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public static class yyConfigurationInterfaceHelper
    {
        // 2025-01-17: Tested with json_edge_cases.json.
        // Duplicate keys caused the parser to throw an exception.
        // Empty arrays were recognized as objects.
        // Key with colons was recognized as a nested object.
        // Everything else seemed fine.

        public static string GetVisibleStrings (this IConfiguration config, string? singleIndent = null, string? newLine = null)
        {
            string xSingleIndent = singleIndent ?? yyString.DefaultSingleIndent,
                   xNewLine = newLine ?? Environment.NewLine;

            StringBuilder xBuilder = new ();

            // IConfiguration is never null.
            xBuilder.Append ($"{{{xNewLine}");

            void _AppendSection (IConfigurationSection section, int indentLevel, bool isArrayElement, bool isLastChild)
            {
                string xFullIndent = xSingleIndent.Repeat (indentLevel),
                       xCommaPart = isLastChild ? "" : ",";

                var xChildren = section.GetChildren ().ToArray ();

                if (xChildren.Length == 0)
                {
                    // Note on null values in IConfiguration:
                    // When the value of a key in IConfiguration is explicitly set to null (e.g., "Key": null in JSON),
                    // IConfiguration interprets and returns it as an empty string ("") instead of null.
                    // This is a design choice in the IConfiguration system, and there is no built-in way to distinguish
                    // between an explicit null value and an empty string in the configuration.
                    // As a result, any key with a null value in the original configuration source will appear as an empty string
                    // when accessed via IConfiguration. This method will output such keys as empty strings, and there is
                    // no workaround to recover the original null value from IConfiguration.

                    if (section.Value != null)
                    {
                        // https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer.serialize
                        string xEscapedValue = JsonSerializer.Serialize (section.Value);

                        if (isArrayElement)
                            xBuilder.Append ($"{xFullIndent}{xEscapedValue}{xCommaPart}{xNewLine}");
                        else xBuilder.Append ($"{xFullIndent}{JsonSerializer.Serialize (section.Key)}: {xEscapedValue}{xCommaPart}{xNewLine}");
                    }

                    else
                    {
                        if (isArrayElement)
                            xBuilder.Append ($"{xFullIndent}{{{xNewLine}");
                        else xBuilder.Append ($"{xFullIndent}{JsonSerializer.Serialize (section.Key)}: {{{xNewLine}");

                        xBuilder.Append ($"{xFullIndent}}}{xCommaPart}{xNewLine}");
                    }
                }

                else
                {
                    // Note on distinguishing numeric keys from arrays:
                    // In IConfiguration, a section with numeric keys (e.g., "0", "1", "2") is typically interpreted as an array.
                    // However, there is no 100% reliable way to distinguish between a genuine array and an object
                    // where all the keys happen to be numeric. For example:
                    //    1. Array representation in JSON:
                    //       "List": [ { "Name": "Alice" }, { "Name": "Bob" } ]
                    //    2. Object with numeric keys:
                    //       "List": { "0": { "Name": "Alice" }, "1": { "Name": "Bob" } }
                    // Both of these structures would appear identical in IConfiguration as sections with numeric keys.
                    //
                    // This code makes a best-effort attempt to distinguish arrays from objects by:
                    //    - Treating sections where all keys are numeric as arrays.
                    //    - Treating sections with mixed or non-numeric keys as objects.
                    // While this heuristic works for most typical configurations, it is not foolproof.
                    // In edge cases where numeric keys are used in non-array contexts, the output may misrepresent objects as arrays.
                    //
                    // If precise differentiation is critical, consider enforcing stricter conventions in the configuration format
                    // (e.g., always representing arrays with numeric keys and objects with non-numeric keys).

                    if (xChildren.All (x => int.TryParse (x.Key, out _)))
                    {
                        if (isArrayElement)
                            xBuilder.Append ($"{xFullIndent}[{xNewLine}");
                        else xBuilder.Append ($"{xFullIndent}{JsonSerializer.Serialize (section.Key)}: [{xNewLine}");

                        for (int temp = 0; temp < xChildren.Length; temp ++)
                        {
                            var xChild = xChildren [temp];
                            _AppendSection (xChild, indentLevel + 1, isArrayElement: true, isLastChild: temp == xChildren.Length - 1);
                        }

                        xBuilder.Append ($"{xFullIndent}]{xCommaPart}{xNewLine}");
                    }

                    else
                    {
                        if (isArrayElement)
                            xBuilder.Append ($"{xFullIndent}{{{xNewLine}");
                        else xBuilder.Append ($"{xFullIndent}{JsonSerializer.Serialize (section.Key)}: {{{xNewLine}");

                        for (int temp = 0; temp < xChildren.Length; temp ++)
                        {
                            var xChild = xChildren [temp];
                            _AppendSection (xChild, indentLevel + 1, isArrayElement: false, isLastChild: temp == xChildren.Length - 1);
                        }

                        xBuilder.Append ($"{xFullIndent}}}{xCommaPart}{xNewLine}");
                    }
                }
            }

            var xRootChildren = config.GetChildren ().ToArray ();

            for (int temp = 0; temp < xRootChildren.Length; temp ++)
            {
                var xChild = xRootChildren [temp];
                _AppendSection (xChild, 1, isArrayElement: false, isLastChild: temp == xRootChildren.Length - 1);
            }

            xBuilder.Append ($"}}{xNewLine}");

            return xBuilder.ToString ();
        }
    }
}
