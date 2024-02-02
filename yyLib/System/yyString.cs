namespace yyLib
{
    public static class yyString
    {
        // c# - ?? Coalesce for empty string? - Stack Overflow
        // https://stackoverflow.com/questions/2420125/coalesce-for-empty-string

        public static string? ConvertNullOrEmptyTo (this string? str, string? defaultString = null) => string.IsNullOrEmpty (str) == false ? str : defaultString;

        public static string? ConvertNullOrWhiteSpaceTo (this string? str, string? defaultString = null) => string.IsNullOrWhiteSpace (str) == false ? str : defaultString;

        public static string GetVisibleString (this string? str)
        {
            if (str == null)
                return "(Null)";

            if (str == string.Empty)
                return "(Empty)";

            return str;
        }

        /// <summary>
        /// For debugging purposes.
        /// </summary>
        public static string GetVisibleLinesString (this string? str, string? newLine = null, string? linePrefix = "^", string? lineSuffix = "$")
        {
            // If only one line returns with a seemingly empty string contained, we never know if it's null or empty.

            if (string.IsNullOrEmpty (str))
                return $"{linePrefix}{str.GetVisibleString ()}{lineSuffix}";

            // If 2 or more lines are returned, we dont need each empty-looking string to be "(Empty)".

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.EnumerateLines (str).Select (x => $"{linePrefix}{x}{lineSuffix}"));
        }

        public static string? TrimEmptyLines (this string? str, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.TrimEmptyLines (str));
        }

        public static string? TrimWhiteSpaceLines (this string? str, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.TrimWhiteSpaceLines (str));
        }
    }
}
