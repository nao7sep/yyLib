using System.Globalization;
using MimeKit;

namespace yyLib
{
    public static class yyConverter
    {
        // In alphabetical order.
        // No extension methods, as they may cause inconsistencies in the method names.
        // All method names must be like: HogeToMoge.

        // -----------------------------------------------------------------------------
        // DateTime
        // -----------------------------------------------------------------------------

        /// <summary>
        /// We get something like: 2023-11-20T19:40:59.1664591Z (UTC) or 2023-11-21T04:40:59.1664591+09:00 (local time).
        /// </summary>
        public static string DateTimeToRoundtripString (DateTime value) =>
            value.ToString ("O", CultureInfo.InvariantCulture);

        /// <summary>
        /// Works with local time too, but we then need to call DateTime.ToUniversalTime.
        /// </summary>
        public static DateTime RoundtripStringToDateTime (string str) =>
            DateTime.ParseExact (str, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        public static bool TryRoundtripStringToDateTime (string str, out DateTime value) =>
            DateTime.TryParseExact (str, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value);

        // Contains enough info to roundtrip, but we wont need Parse for this string format.
        // As time zone indicators' signs in file names may cause unexpected behavior, local time is not supported.

        /// <summary>
        /// Works only with UTC.
        /// </summary>
        public static string DateTimeToRoundtripFileNameString (DateTime utc)
        {
            // Loosely based on the internal roundtrip format: "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#KSpecifier

            // Added: 2024-04-22
            // With a millisecond-level part attached, the core part should still be consistent with the ISO 8601 format.
            // I moved the UTC timezone indicator and deleted the then-redundant hyphen that separated the core part and the rest.

            if (utc.Kind == DateTimeKind.Utc)
                return utc.ToString ("yyyyMMdd'T'HHmmssKfffffff", CultureInfo.InvariantCulture);
            else throw new yyArgumentException ($"'{nameof (utc)}' is not an UTC time: {DateTimeToRoundtripString (utc)}");
        }

        // -----------------------------------------------------------------------------
        // Enum
        // -----------------------------------------------------------------------------

        public static string EnumToString <EnumType> (EnumType value) where EnumType: struct, Enum
        {
            if (Enum.IsDefined (typeof (EnumType), value) == false)
                throw new yyArgumentException ($"Invalid {typeof (EnumType).Name} value: {value}");

            return value.ToString ();
        }

        // It is an option to double.TryParse the string to make sure it is not a string representation of an integral value, but it would affect performance.
        // If the converted value is handled carefully, no security concerns should arise.
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/enums

        public static EnumType StringToEnum <EnumType> (string str, bool ignoreCase = true) where EnumType: struct, Enum
        {
            if (Enum.TryParse <EnumType> (str, ignoreCase: ignoreCase, out EnumType xValue) == false)
                throw new yyArgumentException ($"Invalid {typeof (EnumType).Name} string: {str}");

            return xValue;
        }

        public static bool TryStringToEnum <EnumType> (string str, out EnumType value, bool ignoreCase = true) where EnumType: struct, Enum =>
            Enum.TryParse <EnumType> (str, ignoreCase: ignoreCase, out value);

        public static EnumType ValueToEnum <EnumType> (object value) where EnumType: struct, Enum
        {
            if (Enum.IsDefined (typeof (EnumType), value) == false)
                throw new yyArgumentException ($"Invalid {typeof (EnumType).Name} value: {value}");

            return (EnumType) value;
        }

        public static bool TryValueToEnum <EnumType> (object value, out EnumType enum_value) where EnumType: struct, Enum
        {
            if (Enum.IsDefined (typeof (EnumType), value) == false)
            {
                enum_value = default;
                return false;
            }

            enum_value = (EnumType) value;
            return true;
        }

        // -----------------------------------------------------------------------------
        // Guid
        // -----------------------------------------------------------------------------

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

        // -----------------------------------------------------------------------------
        // MimeMessage
        // -----------------------------------------------------------------------------

        public static byte [] MimeMessageToBytes (MimeMessage message, FormatOptions? options = null, CancellationToken cancellationToken = default)
        {
            using MemoryStream xMemoryStream = new ();

            if (options != null)
                message.WriteTo (options, xMemoryStream, cancellationToken);
            else message.WriteTo (xMemoryStream, cancellationToken);

            return xMemoryStream.ToArray ();
        }
    }
}
