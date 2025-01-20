using System.Text;

namespace yyLib
{
    public static class yyStringHelper
    {
        // c# - ?? Coalesce for empty string? - Stack Overflow
        // https://stackoverflow.com/questions/2420125/coalesce-for-empty-string

        // I, at first, implemented something like ConvertNullOrEmptyTo,
        // but if the method is designed to be called following a possibly null property (like Hoge?.Moge.Convert...),
        // the method may not be called at all and the whole line may return null.
        // It's safer to convert each part to null if necessary and continue using the ?? operator.

        public static string? EmptyToNull (this string? str) => string.IsNullOrEmpty (str) ? null : str;

        public static string? WhiteSpaceToNull (this string? str) => string.IsNullOrWhiteSpace (str) ? null : str;

        public static string GetVisibleString (this string? str)
        {
            if (str == null)
                return "(Null)";

            if (str.Length == 0)
                return "(Empty)";

            // White space characters are not supported as they may contain new lines,
            // which are kind of visible as they affect how things are displayed.

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

            // If 2 or more lines are returned, we dont need each obviously-empty string to be "(Empty)".

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.EnumerateLines (str).Select (x => $"{linePrefix}{x}{lineSuffix}"));
        }

        public static string [] ToLineArray (this string? str)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            return yyStringLines.EnumerateLines (str).ToArray ();
        }

        public static IList <string> ToLineList (this string? str)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            return yyStringLines.EnumerateLines (str).ToList ();
        }

        /// <summary>
        /// Trims considered-empty lines from the beginning and the end of the string.
        /// </summary>
        public static string? TrimRedundantLines (this string? str, yyStringType type = yyStringType.WhiteSpace, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            return string.Join (newLine ?? Environment.NewLine, yyStringLines.TrimRedundantLines (str, type));
        }

        public static string [] ToParagraphArray (this string? str, yyStringType emptyLineType = yyStringType.WhiteSpace, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            return yyStringParagraphs.EnumerateParagraphs (str, emptyLineType).Select (x => string.Join (newLine ?? Environment.NewLine, x)).ToArray ();
        }

        public static IList <string> ToParagraphList (this string? str, yyStringType emptyLineType = yyStringType.WhiteSpace, string? newLine = null)
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

        // Added comment: I've chosen to name this method in a casual way.
        // It doesnt do any more than determining whether each line deserves to remain or not and replacing each sequence of one or more considered-empty lines into a single empty line.
        // A casual and easy-to-remember name would be suitable for a method that does something a little good without causing any trouble and therefore would be called frequently.

        /// <summary>
        /// Splits the string into an enumeration of paragraphs, each of which is a List of lines, by omitting the specified type of lines that are considered "empty," and then reconstructs the entire string.
        /// </summary>
        public static string? Optimize (this string? str, yyStringType type = yyStringType.WhiteSpace, string? newLine = null)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            string xNewLines = newLine != null ? newLine + newLine : Environment.NewLine + Environment.NewLine;
            return string.Join (xNewLines, yyStringParagraphs.EnumerateParagraphs (str, type).Select (x => string.Join (newLine ?? Environment.NewLine, x)));
        }

        // Why "ReplaceAll" is the better name:
        // 1. "ReplaceAll" clearly communicates that all occurrences of the specified characters
        //    in the string will be replaced. There is no ambiguity in the scope of the operation.
        // 2. Familiarity: "ReplaceAll" aligns with common terminology used in programming (e.g.,
        //    ReplaceAll in text editors or similar functions in other languages), making it
        //    intuitive for developers.
        // 3. Avoids Confusion: Alternatives like "ReplaceAny" or "RemoveAny" might imply
        //    selective or partial replacements, leading to misunderstandings about functionality.
        // 4. Consistency: Using "All" emphasizes that the method operates comprehensively,
        //    ensuring clarity and consistency with similar method naming conventions.

        public static string ReplaceAll (this string str, IEnumerable <char> oldChars, char newChar)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            StringBuilder xBuilder = new (str.Length);

            foreach (char xChar in str)
                xBuilder.Append (oldChars.Contains (xChar) ? newChar : xChar);

            return xBuilder.ToString ();
        }

        public static string Repeat (this string str, int count)
        {
            if (string.IsNullOrEmpty (str))
                return str;

            if (count < 0)
                throw new yyArgumentException ("The count must be greater than or equal to zero.");

            if (count == 0)
                return string.Empty; // They held the pen and didnt draw anything rather than they didnt even hold the pen.

            if (count == 1)
                return str;

            return string.Concat (Enumerable.Repeat (str, count));
        }
    }
}
