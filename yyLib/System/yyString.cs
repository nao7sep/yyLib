namespace yyLib
{
    public static class yyString
    {
        // c# - ?? Coalesce for empty string? - Stack Overflow
        // https://stackoverflow.com/questions/2420125/coalesce-for-empty-string

        // I, at first, implemented something like ConvertNullOrEmptyTo,
        //     but if the method is designed to be called following a possibly null property (like Hoge?.Moge.Convert...),
        //     the method may not be called at all and the whole line may return null.
        // It's safer to convert each part to null if necessary and continue using the ?? operator.

        public static string? EmptyToNull (this string? str) => string.IsNullOrEmpty (str) ? null : str;

        public static string? WhiteSpaceToNull (this string? str) => string.IsNullOrWhiteSpace (str) ? null : str;

        public static string GetVisibleString (this string? str)
        {
            if (str == null)
                return "(Null)";

            if (str == string.Empty)
                return "(Empty)";

            return str;
        }

        // This method is for debugging purposes, but it could be used in release builds as well.
        // Plus, even if somebody calls it in production code, it's harmless.

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

        public static string [] ToLineArray (this string? str)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            return yyStringLines.EnumerateLines (str).ToArray ();
        }

        public static List <string> ToLineList (this string? str)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            // Could be faster, but it's not a bottleneck.
            return yyStringLines.EnumerateLines (str).ToList ();
        }

        public static string? TrimLines (this string? str, yyStringType type = yyStringType.WhiteSpace, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.TrimLines (str, type));
        }

        public static string [] ToParagraphArray (this string? str, yyStringType emptyLineType = yyStringType.WhiteSpace, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            return yyStringParagraphs.EnumerateParagraphs (str, emptyLineType).Select (x => string.Join (newLine ?? Environment.NewLine, x)).ToArray ();
        }

        public static List <string> ToParagraphList (this string? str, yyStringType emptyLineType = yyStringType.WhiteSpace, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            return yyStringParagraphs.EnumerateParagraphs (str, emptyLineType).Select (x => string.Join (newLine ?? Environment.NewLine, x)).ToList ();
        }

        // This method could be faster, but performance is not my main concern at the moment.
        // If I allow myself to fine-tune every algorithm I implement, my businesses will never get the software they need. :)

        // Also, I wont be implementing an ability to trim white space chars at the end of each line although I would actually like that.
        // Some people dont mind invisible chars having meanings (like new lines in Markdown) while I personally think it's madness.
        // Some people prefer hard tabs.
        // Some people pay extra attention to horizontally aligning their code by adding/removing spaces while some others use proportional fonts for coding.
        // Some (Japanese) people encourage use of full-width spaces (U+3000) while I mind them because some overseas programs just consider them to be CJK chars and dont treat them as white space.
        // People have very different preferences regarding white space.
        // The only possible consensus, I think, is that strings MAY lose redundant invisible lines at the beginning, inside and at the end.

        /// <summary>
        /// Splits the string into an enumeration of paragraphs, each of which is a List of lines, by omitting specified type of lines that are considered "empty," and then reconstructs the entire string.
        /// </summary>
        public static string? Optimize (this string? str, yyStringType type = yyStringType.WhiteSpace, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            return string.Join (newLine ?? Environment.NewLine, yyStringParagraphs.EnumerateParagraphs (str, type).Select (x => string.Join (newLine ?? Environment.NewLine, x)));
        }
    }
}
