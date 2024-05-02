using Microsoft.Extensions.Configuration;

namespace yyLib
{
    public static class yyAppSettings
    {
        // Configuration in ASP.NET Core | Microsoft Learn
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/

        private static readonly Lazy <IConfiguration> _config = new (() =>
        {
            ConfigurationBuilder xBuilder = new ();

            xBuilder.SetBasePath (yyAppDirectory.Path);

            // appsettings.json: The default configuration file used by the app.
            // Contains settings that are common across all environments.

            xBuilder.AddJsonFile ("appsettings.json", optional: true, reloadOnChange: true);
#if DEBUG
            // appsettings.Development.json: Used when the app is running in a development environment.
            // This file contains development-specific settings (like detailed logging settings, local database connection strings, etc.) that override or supplement the base appsettings.json.

            xBuilder.AddJsonFile ("appsettings.Development.json", optional: true, reloadOnChange: true);

            // appsettings.Staging.json: Used in a staging environment, which is often a pre-production environment used for final testing before deployment to production.
            // This file can include settings closer to what is used in production, such as higher-level logging, staging database connections, etc.

            xBuilder.AddJsonFile ("appsettings.Staging.json", optional: true, reloadOnChange: true);
#endif
            // appsettings.Production.json: Contains configurations specific to the production environment.
            // These settings override the base appsettings.json when the app is running in production.
            // This may include production-level settings such as database connection strings, API keys, logging levels, and other settings optimized for performance and security.

            xBuilder.AddJsonFile ("appsettings.Production.json", optional: true, reloadOnChange: true);

            return xBuilder.Build ();
        });

        public static IConfiguration Config => _config.Value;
    }
}
