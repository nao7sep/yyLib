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

            using StringReader xReader = new (str);
            string? xLine;

            while ((xLine = xReader.ReadLine ()) != null)
                yield return xLine;
        }

        // Sometimes, we receive a string with a lot of new lines attached at the end especially if it's a multi-line user input.
        // Sure, we could just TrimEnd it, but if we also TrimStart it for consistency, the indentation of the first line may be lost.
        // Also, like Markdown, there should be a few languages where invisible space chars at line ends mean something; we cant just TrimEnd each line.
        // So, TrimEmptyLines and TrimWhiteSpaceLines safely eliminate unneeded lines before and after the visible ones.

        private static List <string> TrimLines (string? str, yyStringType type)
        {
            if (string.IsNullOrEmpty (str))
                return [];

            List <string> xLines = [];
            bool xFirstVisibleLineDetected = false;
            List <string> xPending = [];

            foreach (string xLine in EnumerateLines (str))
            {
                if (xFirstVisibleLineDetected == false)
                {
                    if ((type == yyStringType.Empty && string.IsNullOrEmpty (xLine)) ||
                            (type == yyStringType.WhiteSpace && string.IsNullOrWhiteSpace (xLine)))
                        continue;

                    xLines.Add (xLine);
                    xFirstVisibleLineDetected = true;
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

        public static IList <string> TrimEmptyLines (string? str) => TrimLines (str, yyStringType.Empty);

        public static IList <string> TrimWhiteSpaceLines (string? str) => TrimLines (str, yyStringType.WhiteSpace);
    }
}
