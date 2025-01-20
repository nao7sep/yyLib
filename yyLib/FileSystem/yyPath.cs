namespace yyLib
{
    public static class yyPath
    {
        private static readonly char [] _separators = [yyPathSeparators.Windows, yyPathSeparators.Unix];

        public static char [] Separators => _separators;

        private static readonly Lazy <char> _defaultSeparator = new (() =>
        {
            if (yyEnvironment.IsWindows)
                return yyPathSeparators.Windows;
            else if (yyEnvironment.IsUnix)
                return yyPathSeparators.Unix;
            else throw new yyNotSupportedException ("Unsupported operating system.");
        });

        public static char DefaultSeparator => _defaultSeparator.Value;

        public static char GetOtherSeparator (char separator)
        {
            if (separator == yyPathSeparators.Windows)
                return yyPathSeparators.Unix;
            else if (separator == yyPathSeparators.Unix)
                return yyPathSeparators.Windows;
            else throw new ArgumentException ("Invalid separator.");
        }

        private static readonly Lazy <char> _alternativeSeparator = new (() => GetOtherSeparator (DefaultSeparator));

        public static char AlternativeSeparator => _alternativeSeparator.Value;

        public static string NormalizeSeparators (string path, char separator) =>
            path.Replace (GetOtherSeparator (separator), separator);

        public static string Join (char separator, params string [] paths)
        {
            if (paths.Length < 2)
                throw new yyArgumentException ("At least two paths are required.");

            List <string> xTrimmedPaths = [];

            xTrimmedPaths.Add (paths [0].TrimEnd (Separators));
            xTrimmedPaths.AddRange (paths [1..^1].Select (x => x.Trim (Separators)));
            xTrimmedPaths.Add (paths [^1].TrimStart (Separators));

            if (xTrimmedPaths.Any (x => string.IsNullOrEmpty (x)))
                throw new yyArgumentException ("Empty paths are not allowed.");

            string xJoinedPath = string.Join (separator, xTrimmedPaths);

            return NormalizeSeparators (xJoinedPath, separator);
        }

        public static string Join (params string [] paths) => Join (DefaultSeparator, paths);

        public static string GetAbsolutePath (string basePath, string relativePath, char separator)
        {
            if (Path.IsPathFullyQualified (basePath) == false)
                throw new yyArgumentException ("The base path must be fully qualified.");

            if (Path.IsPathFullyQualified (relativePath))
                throw new yyArgumentException ("The relative path must be relative.");

            return Join (separator, basePath, relativePath);
        }

        public static string GetAbsolutePath (string basePath, string relativePath) =>
            GetAbsolutePath (basePath, relativePath, DefaultSeparator);

        // https://github.com/dotnet/runtime/blob/2b60d82ef3e87876128b7f71922a1b72908b6fcf/src/libraries/System.Private.CoreLib/src/System/IO/Path.Windows.cs
        // https://github.com/dotnet/runtime/blob/2b60d82ef3e87876128b7f71922a1b72908b6fcf/src/libraries/System.Private.CoreLib/src/System/IO/Path.Unix.cs

        private static readonly Lazy <char []> _invalidFileNameChars = new (() =>
        {
            List <char> xInvalidFileNameChars =
            [
                '\"', '<', '>', '|', '\0',
                (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
                (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
                (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
                (char)31, ':', '*', '?', '\\', '/'
            ];

            // Making sure.
            // A few more might be added in the future.
            xInvalidFileNameChars.AddRange (Path.GetInvalidFileNameChars ());

            return xInvalidFileNameChars.Distinct ().ToArray ();
        });

        public static char [] InvalidFileNameChars => _invalidFileNameChars.Value;

        private static readonly Lazy <HashSet <char>> _invalidFileNameCharsSet = new (() => new HashSet <char> (InvalidFileNameChars));

        public static HashSet <char> InvalidFileNameCharsSet => _invalidFileNameCharsSet.Value;

        // - is often used to connect words and therefore isnt used as the default replacement character.
        public static string ReplaceAllInvalidFileNameChars (string fileName, char replacement = '_')
        {
            if (string.IsNullOrEmpty (fileName))
                return fileName;

            // Usage of a HashSet, especially a cached one, should make this a little faster.
            return fileName.ReplaceAll (InvalidFileNameCharsSet, replacement);
        }

        private static readonly Lazy <char []> _invalidPathChars = new (() =>
        {
            List <char> xInvalidPathChars =
            [
                '|', '\0',
                (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
                (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
                (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
                (char)31
            ];

            xInvalidPathChars.AddRange (Path.GetInvalidPathChars ());

            return xInvalidPathChars.Distinct ().ToArray ();
        });

        public static char [] InvalidPathChars => _invalidPathChars.Value;

        private static readonly Lazy <HashSet <char>> _invalidPathCharsSet = new (() => new HashSet <char> (InvalidPathChars));

        public static HashSet <char> InvalidPathCharsSet => _invalidPathCharsSet.Value;

        public static string ReplaceAllInvalidPathChars (string path, char replacement = '_')
        {
            if (string.IsNullOrEmpty (path))
                return path;

            return path.ReplaceAll (InvalidPathCharsSet, replacement);
        }
    }
}
