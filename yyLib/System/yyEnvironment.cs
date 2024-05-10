namespace yyLib
{
    public static class yyEnvironment
    {
        private static readonly Lazy <bool> _isNt = new (() => OperatingSystem.IsWindows ());

        public static bool IsNt => _isNt.Value;

        private static readonly Lazy <bool> _isPosix = new (() => OperatingSystem.IsLinux () || OperatingSystem.IsMacOS ());

        // Not much reason to distinguish between Linux and macOS.
        public static bool IsPosix => _isPosix.Value;
    }
}
