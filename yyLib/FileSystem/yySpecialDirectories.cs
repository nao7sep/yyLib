namespace yyLib
{
    public static class yySpecialDirectories
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.environment.specialfolder

        private static readonly Lazy <string> _desktop = new (() => Environment.GetFolderPath (Environment.SpecialFolder.DesktopDirectory)); // The one that gets the physical path.

        // "DesktopDirectory" would be redundant.
        public static string Desktop => _desktop.Value;

        private static readonly Lazy <string> _commonDesktop = new (() => Environment.GetFolderPath (Environment.SpecialFolder.CommonDesktopDirectory));

        public static string CommonDesktop => _commonDesktop.Value;

        // -----------------------------------------------------------------------------

        // The official document says: Applications should not create files or folders at this level; they should put their data under the locations referred to by ApplicationData.
        // But a lot of apps do make directories and files there.

        private static readonly Lazy <string> _userProfile = new (() => Environment.GetFolderPath (Environment.SpecialFolder.UserProfile));

        public static string UserProfile => _userProfile.Value;

        // CommonDocuments points to something like "C:\Users\Public\Documents".
        // As far as I have tested, there doesnt seem to be anything that points to "C:\Users\Public".

        // -----------------------------------------------------------------------------

        private static readonly Lazy <string> _applicationData = new (() => Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData));

        /// <summary>
        /// Store user-specific settings and data that should persist across multiple networked machines here.
        /// </summary>
        public static string ApplicationData => _applicationData.Value;

        private static readonly Lazy <string> _localApplicationData = new (() => Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData));

        /// <summary>
        /// Ideal for storing large or machine-specific user data that doesn't need to be shared across machines.
        /// </summary>
        public static string LocalApplicationData => _localApplicationData.Value;

        private static readonly Lazy <string> _commonApplicationData = new (() => Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData));

        // There's no enum value to get "LocalLow".
        // It's less frequently used and is not implemented for now.

        // ChatGPT says: The "LocalLow" folder in Windows is used as a storage location for data specific to an application that requires a lower level of trust.
        //     It is part of the Windows folder structure that segregates user data into different levels of trust and access.
        //     Specifically, the "LocalLow" folder is often used by applications that run in a more restricted security context,
        //     such as Internet Explorer when it operates in Protected Mode or other applications that run in a low integrity level.

        /// <summary>
        /// Use for application-specific data that is common to all users and does not change frequently.
        /// </summary>
        public static string CommonApplicationData => _commonApplicationData.Value;

        // There seems to be no "common" version of "ApplicationData".
        // Shared info that is roamed across machines anyway doesnt make much sense.
    }
}
