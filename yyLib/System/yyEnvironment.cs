namespace yyLib
{
    public static class yyEnvironment
    {
        private static readonly Lazy <bool> _isWindows = new (OperatingSystem.IsWindows ());

        public static bool IsWindows => _isWindows.Value;

        private static readonly Lazy <bool> _isUnix = new (OperatingSystem.IsLinux () || OperatingSystem.IsMacOS ());

        // Not much reason to distinguish between Linux and macOS.
        public static bool IsUnix => _isUnix.Value;
    }
}
