using System.Reflection;

namespace yyLib
{
    // GetEntryAssembly returns RuntimeAssembly, that is an internal, derived class of Assembly.
    // We cant convert RuntimeAssembly to yyAssembly: Assembly.
    // The class doesnt inherit from Assembly and, instead, contains a corresponding property.
    // The name contains "Info" because the class is more like a information extractor (like FileInfo).
    // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Reflection/Assembly.cs
    // https://source.dot.net/#System.Private.CoreLib/src/System/Reflection/RuntimeAssembly.cs

    public class yyAssemblyInfo (Assembly assembly)
    {
        public Assembly Assembly { get; init; } = assembly;

        // -----------------------------------------------------------------------------
        //     Used Frequently
        // -----------------------------------------------------------------------------

        public string Location => Assembly.Location;

        /// <summary>
        /// Returns something like: yyLibConsole, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
        /// </summary>
        public string? FullName => Assembly.FullName;

        // -----------------------------------------------------------------------------
        //     From Metadata
        // -----------------------------------------------------------------------------

        private string? _assemblyName;

        /// <summary>
        /// Used as the output file name. Refer to AssemblyTitle for the assembly's display name.
        /// </summary>
        public string? AssemblyName => _assemblyName ??= Assembly.GetName ().Name;

        private string? _assemblyTitle;

        /// <summary>
        /// The assembly's display name. Refer to this rather than AssemblyName, that is the output file name.
        /// </summary>
        public string? AssemblyTitle => _assemblyTitle ??= Assembly.GetCustomAttribute <AssemblyTitleAttribute> ()?.Title;

        private string? _company;

        public string? Company => _company ??= Assembly.GetCustomAttribute <AssemblyCompanyAttribute> ()?.Company;

        private string? _product;

        public string? Product => _product ??= Assembly.GetCustomAttribute <AssemblyProductAttribute> ()?.Product;

        private string? _description;

        public string? Description => _description ??= Assembly.GetCustomAttribute <AssemblyDescriptionAttribute> ()?.Description;

        private string? _copyright;

        public string? Copyright => _copyright ??= Assembly.GetCustomAttribute <AssemblyCopyrightAttribute> ()?.Copyright;

        private string? _configuration;

        /// <summary>
        /// You may not get what you expect from the .csproj file because the compiler may override it.
        /// </summary>
        public string? Configuration => _configuration ??= Assembly.GetCustomAttribute <AssemblyConfigurationAttribute> ()?.Configuration;

        // Versions are confusing.

        // "Version" was originally for NuGet.
        // Some developers incorrectly assumed it's a newly-introduced general-purpose attribute.
        // The right attribute for the assembly's version is and has been "AssemblyVersion".
        // For the developers' convenience, the .NET team made "Version" and "AssemblyVersion" interchangeable.

        // For some reason, AssemblyVersionAttribute doesnt seem to work.
        // Assembly.GetName ().Version?.ToString () seems to be the only straight-forward way to get "Version" and "AssemblyVersion".
        // If both the attributes are specified, this method returns "AssemblyVersion".
        // In this situation, if you still want "Version", you'll probably have to parse the .csproj file.

        // "Version" is more intuitive than "AssemblyVersion".
        // It may also be useful when the assembly is released as a NuGet package because we wont be needing to update 2 version strings.
        // The best practice should be: Specifying "Version" and programmatically referring to the AssemblyVersion property.

        private string? _version;

        /// <summary>
        /// The NuGet package version. Refer to AssemblyVersion for the assembly's version.
        /// </summary>
        public string? Version => _version ??= Assembly.GetName ().Version?.ToString ();

        private string? _assemblyVersion;

        /// <summary>
        /// The assembly's version. Refer to this rather than Version, that is the NuGet package version.
        /// </summary>
        public string? AssemblyVersion => _assemblyVersion ??= Assembly.GetName ().Version?.ToString ();

        private string? _fileVersion;

        public string? FileVersion => _fileVersion ??= Assembly.GetCustomAttribute <AssemblyFileVersionAttribute> ()?.Version;

        private string? _informationalVersion;

        /// <summary>
        /// Returns something like: 0.1+03073b01cc6d77db9dfaab1b7411d7a940e54eb2
        /// </summary>
        public string? InformationalVersion => _informationalVersion ??= Assembly.GetCustomAttribute <AssemblyInformationalVersionAttribute> ()?.InformationalVersion;

        /// <summary>
        /// For debugging purposes.
        /// </summary>
        public string? GetVisibleStrings (string? newLine = null) // Plural.
        {
            List <string> xLines = new ();

            xLines.Add ($"Location: {Location.GetVisibleString ()}");
            xLines.Add ($"FullName: {FullName.GetVisibleString ()}");
            xLines.Add ($"AssemblyName: {AssemblyName.GetVisibleString ()}");
            xLines.Add ($"AssemblyTitle: {AssemblyTitle.GetVisibleString ()}");
            xLines.Add ($"Company: {Company.GetVisibleString ()}");
            xLines.Add ($"Product: {Product.GetVisibleString ()}");
            xLines.Add ($"Description: {Description.GetVisibleString ()}");
            xLines.Add ($"Copyright: {Copyright.GetVisibleString ()}");
            xLines.Add ($"Configuration: {Configuration.GetVisibleString ()}");
            xLines.Add ($"Version: {Version.GetVisibleString ()}");
            xLines.Add ($"AssemblyVersion: {AssemblyVersion.GetVisibleString ()}");
            xLines.Add ($"FileVersion: {FileVersion.GetVisibleString ()}");
            xLines.Add ($"InformationalVersion: {InformationalVersion.GetVisibleString ()}");

            return string.Join (newLine ?? Environment.NewLine, xLines);
        }

        // -----------------------------------------------------------------------------
        //     Static Properties
        // -----------------------------------------------------------------------------

        private static readonly Lazy <yyAssemblyInfo?> _appAssembly = new (() =>
        {
            Assembly? xAssembly = Assembly.GetEntryAssembly ();

            if (xAssembly != null)
                return new (xAssembly);

            return null;
        });

        public static yyAssemblyInfo? AppAssembly => _appAssembly.Value;

        private static readonly Lazy <yyAssemblyInfo> _libraryAssembly = new (() =>
        {
            Assembly xAssembly = Assembly.GetExecutingAssembly (); // Not nullable.
            return new (xAssembly);
        });

        public static yyAssemblyInfo LibraryAssembly => _libraryAssembly.Value;
    }
}
