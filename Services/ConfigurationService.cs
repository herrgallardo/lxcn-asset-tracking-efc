using Microsoft.Extensions.Configuration;

namespace lxcn_asset_tracking_efc.Services
{
    /// <summary>
    /// Service for managing application configuration including database connection strings
    /// </summary>
    public class ConfigurationService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor that initializes the configuration service
        /// Loads settings from appsettings.json
        /// </summary>
        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        /// <summary>
        /// Gets the database connection string from configuration
        /// </summary>
        /// <returns>SQL Server connection string</returns>
        /// <exception cref="InvalidOperationException">Thrown when connection string is not found</exception>
        public string GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string not found. Please ensure appsettings.json contains a valid ConnectionStrings:DefaultConnection value.");
            }

            return connectionString;
        }

        /// <summary>
        /// Gets a configuration value by key
        /// </summary>
        /// <param name="key">Configuration key to retrieve</param>
        /// <returns>Configuration value or null if not found</returns>
        public string? GetValue(string key)
        {
            return _configuration[key];
        }
    }
}