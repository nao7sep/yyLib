using System.Globalization;

namespace yyLib
{
    public static class yyFormatter
    {
        public static string ToRoundtripString (this DateTime value) =>
            value.ToString ("O", CultureInfo.InvariantCulture);
        // todo: Check documents.
        // todo: Test with local time.
        // todo: Use "utc" as the parameter name if it works only with UTC.

        public static DateTime ParseRoundtripDateTimeString (string str) =>
            DateTime.ParseExact (str, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        // todo: Do the same.

        public static string ToRoundtripFileNameString (this DateTime value) =>
            value.ToString ("yyyyMMdd'T'HHmmss'-'fffffff'Z'", CultureInfo.InvariantCulture);
        // todo: Check the format string.
        // todo: Test with a low precision value.
        // todo: Comment why this format.
    }
}
