using System.Globalization;

namespace yyLib
{
    public static class yyFormatter
    {
        /// <summary>
        /// We get something like: 2023-11-20T19:40:59.1664591Z (UTC) or 2023-11-21T04:40:59.1664591+09:00 (local time).
        /// </summary>
        public static string ToRoundtripString (this DateTime value) =>
            value.ToString ("O", CultureInfo.InvariantCulture);

        /// <summary>
        /// Works with local time too, but we then need to call DateTime.ToUniversalTime.
        /// </summary>
        public static DateTime ParseRoundtripDateTimeString (string str) =>
            DateTime.ParseExact (str, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        // Contains enough info to roundtrip, but we wont need Parse for this.
        // As time zone indicators' signs in file names may cause unexpected behavior, local time is not supported.

        /// <summary>
        /// Works only with UTC.
        /// </summary>
        public static string ToRoundtripFileNameString (this DateTime utc)
        {
            // Loosely based on the internal roundtrip format: "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#KSpecifier

            if (utc.Kind == DateTimeKind.Utc)
                return utc.ToString ("yyyyMMdd'T'HHmmss'-'fffffffK", CultureInfo.InvariantCulture);

            else throw new yyArgumentException (yyMessage.Create ($"'{nameof (utc)}' is not an UTC time: {utc.ToRoundtripString ()}"));
        }
    }
}
