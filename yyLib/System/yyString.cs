namespace yyLib
{
    public static class yyString
    {
        // todo: Test with various strings.

        public static string? TrimEmptyLines (this string? str, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.TrimEmptyLines (str));
        }

        // todo: Test with various strings.

        public static string? TrimWhiteSpaceLines (this string? str, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.TrimWhiteSpaceLines (str));
        }
    }
}
