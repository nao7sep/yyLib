namespace yyLib
{
    public static class yyPathSeparators
    {
        // These used to be "Nt" and "Posix."
        // Considering the following file names, "Windows" and "Unix" would be OK, more intuitive and "Unix" should safely cover MacOS as well.
        // https://github.com/dotnet/runtime/blob/dae890906431049d32e24d498a1d707a441a64a8/src/libraries/System.Private.CoreLib/src/System/IO/Path.Windows.cs
        // https://github.com/dotnet/runtime/blob/dae890906431049d32e24d498a1d707a441a64a8/src/libraries/System.Private.CoreLib/src/System/IO/Path.Unix.cs
        // https://github.com/dotnet/runtime/blob/dae890906431049d32e24d498a1d707a441a64a8/src/libraries/System.Private.CoreLib/src/System/IO/Path.Unix.iOS.cs

        public static char Windows => '\\';

        public static char Unix => '/';
    }
}
