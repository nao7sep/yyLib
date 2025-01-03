﻿using System.Globalization;

namespace yyLib
{
    public static class yyFormatter
    {
        // The method name doesnt need to contain "DateTime" as it's obvious from the parameter type.

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

        public static bool TryParseRoundtripDateTimeString (string str, out DateTime value) =>
            DateTime.TryParseExact (str, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value);

        // Contains enough info to roundtrip, but we wont need Parse for this string format.
        // As time zone indicators' signs in file names may cause unexpected behavior, local time is not supported.

        /// <summary>
        /// Works only with UTC.
        /// </summary>
        public static string ToRoundtripFileNameString (this DateTime utc)
        {
            // Loosely based on the internal roundtrip format: "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#KSpecifier

            // Added: 2024-04-22
            // With a millisecond-level part attached, the core part should still be consistent with the ISO 8601 format.
            // I moved the UTC timezone indicator and deleted the then-redundant hyphen that separated the core part and the rest.

            if (utc.Kind == DateTimeKind.Utc)
                return utc.ToString ("yyyyMMdd'T'HHmmssKfffffff", CultureInfo.InvariantCulture);

            else throw new yyArgumentException ($"'{nameof (utc)}' is not an UTC time: {utc.ToRoundtripString ()}");
        }

        // https://learn.microsoft.com/en-us/dotnet/api/system.guid.tostring
        // https://learn.microsoft.com/en-us/dotnet/api/system.guid.parse
        // https://learn.microsoft.com/en-us/dotnet/api/system.guid.tryparse

        /// <summary>
        /// 32 digits separated by hyphens: 00000000-0000-0000-0000-000000000000
        /// </summary>
        public static string GuidToString (Guid guid) => guid.ToString ("D");

        /// <summary>
        /// Should parse all variations case-insensitively.
        /// </summary>
        public static Guid StringToGuid (string str) => Guid.Parse (str);

        /// <summary>
        /// Should parse all variations case-insensitively.
        /// </summary>
        public static bool TryStringToGuid (string str, out Guid guid) => Guid.TryParse (str, out guid);
    }
}
