using System.Text;
using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public static class yyConfigurationInterfaceHelper
    {
        public static string GetVisibleStrings (this IConfiguration config, string? singleIndent = null, string? newLine = null)
        {
            string xSingleIndent = singleIndent ?? yyString.DefaultSingleIndent,
                   xNewLine = newLine ?? Environment.NewLine;

            StringBuilder xBuilder = new ();

            // IConfiguration is never null.
            xBuilder.Append ($"{{{xNewLine}");

            void _AppendSection (IConfigurationSection section, int indentLevel, bool isLastChild)
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
                        // Note on escaping special characters:
                        // This function escapes only backslashes ("\") and double quotes ("\"") in values to ensure the output remains
                        // readable and somewhat structured. While there are additional characters that should be escaped to produce
                        // fully valid JSON (e.g., control characters like \b, \f, \n, \r, \t), handling these would complicate the code
                        // and is unnecessary for this function's primary purpose, which is debugging.
                        // Since this function is not intended to produce strictly valid JSON, the limited escaping is sufficient for
                        // readability and debugging needs.
                        // If strict JSON compliance or escaping of all special characters is required, consider using a JSON library
                        // like System.Text.Json or Newtonsoft.Json instead.

                        string xEscapedValue = section.Value.Replace ("\\", "\\\\").Replace ("\"", "\\\"");
                        xBuilder.Append ($"{xFullIndent}\"{section.Key}\": \"{xEscapedValue}\"{xCommaPart}{xNewLine}");
                    }

                    else
                    {
                        xBuilder.Append ($"{xFullIndent}\"{section.Key}\": {{{xNewLine}");
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
                        // Note on arrays with numeric keys and null values:
                        // This method outputs arrays with numeric keys (e.g., "0", "1") instead of converting them into valid JSON arrays.
                        // While this is not compliant with JSON standards (where arrays must be enclosed in [ ] without keys),
                        // it reflects the actual data structure within IConfiguration, which uses a flattened representation where
                        // arrays are stored as sections with numeric keys.
                        //
                        // Similarly, this method does not attempt to restore null values for keys where IConfiguration has
                        // automatically converted null to an empty string (""). These behaviors are intentional because the purpose
                        // of this method is to provide a faithful view of the data as represented in IConfiguration for debugging.
                        // By outputting the raw structure, including numeric keys for arrays and empty strings for nulls,
                        // this method helps developers understand the exact state of the IConfiguration hierarchy they are working with.
                        //
                        // This means:
                        //    - Arrays will be displayed as objects with numeric keys (e.g., { "0": ..., "1": ... }).
                        //    - Null values will be displayed as empty strings ("").
                        //
                        // For producing strictly valid JSON or restoring null values, a different approach would be needed.

                        xBuilder.Append ($"{xFullIndent}\"{section.Key}\": [{xNewLine}");

                        for (int temp = 0; temp < xChildren.Length; temp ++)
                        {
                            var xChild = xChildren [temp];
                            _AppendSection (xChild, indentLevel + 1, temp == xChildren.Length - 1);
                        }

                        xBuilder.Append ($"{xFullIndent}]{xCommaPart}{xNewLine}");
                    }

                    else
                    {
                        xBuilder.Append ($"{xFullIndent}\"{section.Key}\": {{{xNewLine}");

                        for (int temp = 0; temp < xChildren.Length; temp ++)
                        {
                            var xChild = xChildren [temp];
                            _AppendSection (xChild, indentLevel + 1, temp == xChildren.Length - 1);
                        }

                        xBuilder.Append ($"{xFullIndent}}}{xCommaPart}{xNewLine}");
                    }
                }
            }

            var xRootChildren = config.GetChildren ().ToArray ();

            for (int temp = 0; temp < xRootChildren.Length; temp ++)
            {
                var xChild = xRootChildren [temp];
                _AppendSection (xChild, 1, temp == xRootChildren.Length - 1);
            }

            xBuilder.Append ($"}}{xNewLine}");

            return xBuilder.ToString ();
        }
    }
}
