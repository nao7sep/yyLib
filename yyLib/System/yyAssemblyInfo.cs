using System.Reflection;

namespace yyLib
{
    // GetEntryAssembly returns RuntimeAssembly, that is an internal, derived class of Assembly.
    // We cant convert RuntimeAssembly to yyAssembly: Assembly.
    // The class doesnt inherit from Assembly and, instead, contains a corresponding property.
    // The name contains "Info" because the class is more like a information extractor (like FileInfo).
    // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Reflection/Assembly.cs
    // https://source.dot.net/#System.Private.CoreLib/src/System/Reflection/RuntimeAssembly.cs

    public class yyAssemblyInfo
    {
        public Assembly Assembly { get; init; }

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

        // The initialization of some of the following Lazy properties occur in the constructor
        // because they depend on the Assembly property, that is unavailable upon the class's instantiation.

        private readonly Lazy <string?> _assemblyName;

        /// <summary>
        /// Used as the output file name. Refer to AssemblyTitle for the assembly's display name.
        /// </summary>
        public string? AssemblyName => _assemblyName.Value;

        private readonly Lazy <string?> _assemblyTitle;

        /// <summary>
        /// The assembly's display name. Refer to this rather than AssemblyName, that is the output file name.
        /// </summary>
        public string? AssemblyTitle => _assemblyTitle.Value ?? AssemblyName; // Intuitive fallback.

        private readonly Lazy <string?> _company;

        public string? Company => _company.Value;

        private readonly Lazy <string?> _product;

        public string? Product => _product.Value;

        private readonly Lazy <string?> _description;

        public string? Description => _description.Value;

        private readonly Lazy <string?> _copyright;

        public string? Copyright => _copyright.Value;

        private readonly Lazy <string?> _configuration;

        /// <summary>
        /// You may not get what you expect from the .csproj file because the compiler may override it.
        /// </summary>
        public string? Configuration => _configuration.Value;

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

        private readonly Lazy <Version?> _version;

        /// <summary>
        /// The NuGet package version. Refer to AssemblyVersion for the assembly's version.
        /// </summary>
        public Version? Version => _version.Value;

        private readonly Lazy <Version?> _assemblyVersion;

        /// <summary>
        /// The assembly's version. Refer to this rather than Version, that is the NuGet package version.
        /// </summary>
        public Version? AssemblyVersion => _assemblyVersion.Value;

        private readonly Lazy <string?> _fileVersion;

        public string? FileVersion => _fileVersion.Value;

        private readonly Lazy <string?> _informationalVersion;

        /// <summary>
        /// Returns something like: 0.1+03073b01cc6d77db9dfaab1b7411d7a940e54eb2
        /// </summary>
        public string? InformationalVersion => _informationalVersion.Value;

        public yyAssemblyInfo (Assembly assembly)
        {
            Assembly = assembly;

            _assemblyName = new (() => Assembly.GetName ().Name);
            _assemblyTitle = new (() => Assembly.GetCustomAttribute <AssemblyTitleAttribute> ()?.Title);
            _company = new (() => Assembly.GetCustomAttribute <AssemblyCompanyAttribute> ()?.Company);
            _product = new (() => Assembly.GetCustomAttribute <AssemblyProductAttribute> ()?.Product);
            _description = new (() => Assembly.GetCustomAttribute <AssemblyDescriptionAttribute> ()?.Description);
            _copyright = new (() => Assembly.GetCustomAttribute <AssemblyCopyrightAttribute> ()?.Copyright);
            _configuration = new (() => Assembly.GetCustomAttribute <AssemblyConfigurationAttribute> ()?.Configuration);
            _version = new (() => Assembly.GetName ().Version);
            _assemblyVersion = new (() => Assembly.GetName ().Version);
            _fileVersion = new (() => Assembly.GetCustomAttribute <AssemblyFileVersionAttribute> ()?.Version);
            _informationalVersion = new (() => Assembly.GetCustomAttribute <AssemblyInformationalVersionAttribute> ()?.InformationalVersion);
        }

        /// <summary>
        /// For debugging purposes.
        /// </summary>
        public string? GetVisibleStrings (string? newLine = null) // Plural.
        {
            List <string> xLines = new ();

            // Version.ToString refers to DefaultFormatFieldCount and generates a 2-3 field string if the trailing fields are not specified.
            // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Version.cs

            xLines.Add ($"Location: {Location.GetVisibleString ()}");
            xLines.Add ($"FullName: {FullName.GetVisibleString ()}");
            xLines.Add ($"AssemblyName: {AssemblyName.GetVisibleString ()}");
            xLines.Add ($"AssemblyTitle: {AssemblyTitle.GetVisibleString ()}");
            xLines.Add ($"Company: {Company.GetVisibleString ()}");
            xLines.Add ($"Product: {Product.GetVisibleString ()}");
            xLines.Add ($"Description: {Description.GetVisibleString ()}");
            xLines.Add ($"Copyright: {Copyright.GetVisibleString ()}");
            xLines.Add ($"Configuration: {Configuration.GetVisibleString ()}");
            xLines.Add ($"Version: {(Version?.ToString ()).GetVisibleString ()}");
            xLines.Add ($"AssemblyVersion: {(AssemblyVersion?.ToString ()).GetVisibleString ()}");
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
