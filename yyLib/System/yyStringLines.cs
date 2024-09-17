namespace yyLib
{
    public static class yyStringLines
    {
        public static IEnumerable <string> EnumerateLines (string? str)
        {
            // Console.Write (new StringReader ("").ReadLine () == null) displays True.
            // If 'str' is null or empty, it must be an empty enumerator.

            if (string.IsNullOrEmpty (str))
                yield break;

            // This instance should be disposed even if EnumerateLines is used in a foreach loop and the loop is exited early.
            using StringReader xReader = new (str);
            string? xLine;

            while ((xLine = xReader.ReadLine ()) != null)
                yield return xLine;
        }

        // Sometimes, we receive a string with a lot of new lines attached at the end especially if it's a multi-line user input.
        // Sure, we could just TrimEnd it, but if we also TrimStart it for consistency, the indentation of the first line may be lost.
        // Also, like Markdown, there should be a few languages where invisible space chars at line ends mean something; we cant just TrimEnd each line.
        // So, TrimEmptyLines and TrimWhiteSpaceLines safely eliminate unneeded lines before and after the visible ones.

        // Additional comment: This is a multiline version of Trim just as the name suggests, meaning internal redundant empty lines are preserved.
        // The type parameter specifies what type of lines are considered "unnecessary" and must be "trimmed" from the beginning and the end of the string.

        // Additional comments again: The method has been renamed from TrimLines to TrimRedundantLines.
        // This method omits considered-empty lines from the beginning and the end of the string.
        // "TrimLines" may sound like it trims EACH line, but it actually trims the whole string considering it as a sequence of lines.

        /// <summary>
        /// Trims considered-empty lines from the beginning and the end of the string.
        /// </summary>
        public static List <string> TrimRedundantLines (string? str, yyStringType type)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            List <string> xLines = [];
            bool xIsFirstVisibleLineDetected = false;
            List <string> xPending = [];

            foreach (string xLine in EnumerateLines (str))
            {
                if (xIsFirstVisibleLineDetected == false)
                {
                    if ((type == yyStringType.Empty && string.IsNullOrEmpty (xLine)) ||
                            (type == yyStringType.WhiteSpace && string.IsNullOrWhiteSpace (xLine)))
                        continue;

                    xLines.Add (xLine);
                    xIsFirstVisibleLineDetected = true;
                }

                else
                {
                    if ((type == yyStringType.Empty && string.IsNullOrEmpty (xLine)) ||
                        (type == yyStringType.WhiteSpace && string.IsNullOrWhiteSpace (xLine)))
                    {
                        xPending.Add (xLine);
                        continue;
                    }

                    xLines.AddRange (xPending);
                    xPending.Clear ();
                    xLines.Add (xLine);
                }
            }

            return xLines;
        }
    }
}
