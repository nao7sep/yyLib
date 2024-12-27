namespace yyLib
{
    public static class yyStringParagraphs
    {
        public static IEnumerable <List <string>> EnumerateParagraphs (string? str, yyStringType emptyLineType = yyStringType.WhiteSpace)
        {
            if (string.IsNullOrEmpty (str))
                yield break;

            using StringReader xReader = new (str);
            string? xLine;

            List <string> xParagraph = new ();
            bool xIsFirstVisibleLineDetected = false;

            while ((xLine = xReader.ReadLine ()) != null)
            {
                if (xIsFirstVisibleLineDetected == false)
                {
                    if ((emptyLineType == yyStringType.Empty && string.IsNullOrEmpty (xLine)) ||
                            (emptyLineType == yyStringType.WhiteSpace && string.IsNullOrWhiteSpace (xLine)))
                        continue;

                    // The first visible line is detected.
                    xParagraph.Add (xLine);
                    xIsFirstVisibleLineDetected = true;
                }

                else
                {
                    if ((emptyLineType == yyStringType.Empty && string.IsNullOrEmpty (xLine)) ||
                        (emptyLineType == yyStringType.WhiteSpace && string.IsNullOrWhiteSpace (xLine)))
                    {
                        if (xParagraph.Count > 0)
                        {
                            yield return xParagraph;
                            xParagraph = new ();
                        }

                        continue;
                    }

                    // Another visible line is detected.
                    xParagraph.Add (xLine);
                }
            }

            if (xParagraph.Count > 0)
                yield return xParagraph;
        }
    }
}
