using Microsoft.EntityFrameworkCore;
using lxcn_asset_tracking_efc.Data;
using lxcn_asset_tracking_efc.Services;
using lxcn_asset_tracking_efc.Models;

namespace lxcn_asset_tracking_efc
{
    /// <summary>
    /// Main program class for the Asset Tracking application
    /// Enhanced with currency conversion functionality matching the original GitHub implementation
    /// </summary>
    class Program
    {
        private static AssetManager? _assetManager;
        private static DisplayService? _displayService;
        private static InputService? _inputService;

        /// <summary>
        /// Main entry point of the application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static async Task Main(string[] args)
        {
            Console.WriteLine("Asset Tracking System");
            Console.WriteLine("=====================\n");

            if (!await InitializeApplicationAsync())
            {
                Console.WriteLine("Failed to initialize application. Press any key to exit...");
                Console.ReadKey();
                return;
            }

            // Initialize currency converter like in the original implementation
            await InitializeCurrencyConverterAsync();

            // Ask user if they want to add sample data
            Console.Write("\nWould you like to add sample data for testing? (y/n): ");
            var addSampleData = Console.ReadLine()?.Trim().ToLower();

            if (addSampleData == "y" || addSampleData == "yes")
            {
                await AddSampleDataAsync();
            }

            await RunApplicationAsync();
        }

        /// <summary>
        /// Initializes the currency converter with live exchange rates
        /// </summary>
        private static async Task InitializeCurrencyConverterAsync()
        {
            Console.WriteLine("Initializing currency converter...");

            try
            {
                // Initialize currency converter with error suppression
                var updateSuccess = await Task.Run(() => CurrencyConverter.Update(true));

                if (updateSuccess && CurrencyConverter.HasValidRates())
                {
                    Console.WriteLine("Currency rates updated successfully from European Central Bank.");
                    Console.WriteLine($"Last updated: {CurrencyConverter.GetLastUpdateTime():yyyy-MM-dd HH:mm}");
                }
                else
                {
                    Console.WriteLine("Warning: Using fallback currency rates.");
                    Console.WriteLine("Exchange rates from ECB unavailable - using approximate rates:");
                    Console.WriteLine("1 EUR = 1.10 USD");
                    Console.WriteLine("1 EUR = 10.50 SEK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not update currency rates: {ex.Message}");
                Console.WriteLine("Will use default conversion rates.");
            }
        }

        /// <summary>
        /// Initializes the application with database context and services
        /// </summary>
        /// <returns>True if initialization successful, false otherwise</returns>
        private static async Task<bool> InitializeApplicationAsync()
        {
            try
            {
                // Initialize configuration service
                var configService = new ConfigurationService();
                var connectionString = configService.GetConnectionString();

                // Configure DbContext options
                var optionsBuilder = new DbContextOptionsBuilder<AssetTrackingContext>();
                optionsBuilder.UseSqlServer(connectionString);

                // Create database context
                var context = new AssetTrackingContext(optionsBuilder.Options);

                // Ensure database is created and up to date
                Console.WriteLine("Checking database connection...");
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("Database connection successful.");

                // Initialize services
                _assetManager = new AssetManager(context);
                _displayService = new DisplayService();
                _inputService = new InputService(_displayService);

                Console.WriteLine("Application initialized successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing application: {ex.Message}");
                Console.WriteLine("\nPlease check your database connection string in appsettings.json");
                return false;
            }
        }

        /// <summary>
        /// Adds sample data to the database for testing purposes
        /// </summary>
        private static async Task AddSampleDataAsync()
        {
            var sampleDataService = new SampleDataService(_assetManager!);
            await sampleDataService.AddSampleDataAsync();

            Console.Write("Would you also like to add end-of-life test data? (y/n): ");
            var addTestData = Console.ReadLine()?.Trim().ToLower();

            if (addTestData == "y" || addTestData == "yes")
            {
                await sampleDataService.AddEndOfLifeTestDataAsync();
            }

            Console.WriteLine("Sample data setup complete!");
            Console.WriteLine("Press any key to continue to the main menu...");
            Console.ReadKey();
        }

        /// <summary>
        /// Main application loop with menu-driven interface
        /// </summary>
        private static async Task RunApplicationAsync()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                _displayService!.DisplayMainMenu();

                var choice = _inputService!.GetIntInput("", 1, 6);

                switch (choice)
                {
                    case 1:
                        await ViewAllAssetsAsync();
                        break;
                    case 2:
                        await AddNewAssetAsync();
                        break;
                    case 3:
                        await UpdateAssetAsync();
                        break;
                    case 4:
                        await DeleteAssetAsync();
                        break;
                    case 5:
                        await ViewReportsAsync();
                        break;
                    case 6:
                        running = false;
                        Console.WriteLine("Thank you for using Asset Tracking System!");
                        break;
                    default:
                        _displayService.DisplayError("Invalid menu selection.");
                        _inputService.PauseForUser();
                        break;
                }
            }
        }

        /// <summary>
        /// Displays all assets sorted by office location and purchase date
        /// </summary>
        private static async Task ViewAllAssetsAsync()
        {
            Console.Clear();
            Console.WriteLine("Loading assets...\n");

            var assets = await _assetManager!.GetAssetsSortedAsync();
            _displayService!.DisplayAssets(assets);

            _inputService!.PauseForUser();
        }

        /// <summary>
        /// Handles adding a new asset to the database
        /// </summary>
        private static async Task AddNewAssetAsync()
        {
            Console.Clear();

            var asset = _inputService!.CreateAsset();
            if (asset == null)
            {
                _displayService!.DisplayError("Asset creation cancelled.");
                _inputService.PauseForUser();
                return;
            }

            Console.WriteLine("\nAsset details:");
            Console.WriteLine($"Type: {asset.AssetType}");
            Console.WriteLine($"Brand: {asset.Brand}");
            Console.WriteLine($"Model: {asset.Model}");
            Console.WriteLine($"Purchase Date: {asset.PurchaseDate.ToShortDateString()}");
            Console.WriteLine($"Price: {asset.PurchasePrice}");
            Console.WriteLine($"USD Equivalent: ${asset.PurchasePrice.ToUSD():N2}");
            Console.WriteLine($"Office: {asset.OfficeLocation}");

            if (_inputService.GetConfirmation("\nConfirm adding this asset?"))
            {
                var success = await _assetManager!.AddAssetAsync(asset);
                if (success)
                {
                    _displayService!.DisplaySuccess("Asset added successfully!");
                }
                else
                {
                    _displayService!.DisplayError("Failed to add asset.");
                }
            }
            else
            {
                _displayService!.DisplayWarning("Asset addition cancelled.");
            }

            _inputService.PauseForUser();
        }

        /// <summary>
        /// Handles updating an existing asset
        /// </summary>
        private static async Task UpdateAssetAsync()
        {
            Console.Clear();

            var assets = await _assetManager!.GetAllAssetsAsync();
            if (assets.Count == 0)
            {
                _displayService!.DisplayWarning("No assets available to update.");
                _inputService!.PauseForUser();
                return;
            }

            _displayService!.DisplayAssets(assets);

            var assetId = _inputService!.GetIntInput("\nEnter Asset ID to update: ", 1);
            if (assetId == -1) return;

            var asset = await _assetManager.GetAssetByIdAsync(assetId);
            if (asset == null)
            {
                _displayService.DisplayError("Asset not found.");
                _inputService.PauseForUser();
                return;
            }

            Console.WriteLine($"\nCurrent details for {asset.AssetType} ID {asset.Id}:");
            Console.WriteLine($"Brand: {asset.Brand}");
            Console.WriteLine($"Model: {asset.Model}");
            Console.WriteLine($"Office: {asset.OfficeLocation}");

            // Simple update - just allow changing office location
            Console.Write($"\nEnter new office location (current: {asset.OfficeLocation}): ");
            var newOffice = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(newOffice) && newOffice != asset.OfficeLocation)
            {
                asset.OfficeLocation = newOffice;
                var success = await _assetManager.UpdateAssetAsync(asset);

                if (success)
                {
                    _displayService.DisplaySuccess("Asset updated successfully!");
                }
                else
                {
                    _displayService.DisplayError("Failed to update asset.");
                }
            }
            else
            {
                _displayService.DisplayWarning("No changes made.");
            }

            _inputService.PauseForUser();
        }

        /// <summary>
        /// Handles deleting an asset from the database
        /// </summary>
        private static async Task DeleteAssetAsync()
        {
            Console.Clear();

            var assets = await _assetManager!.GetAllAssetsAsync();
            if (assets.Count == 0)
            {
                _displayService!.DisplayWarning("No assets available to delete.");
                _inputService!.PauseForUser();
                return;
            }

            _displayService!.DisplayAssets(assets);

            var assetId = _inputService!.GetIntInput("\nEnter Asset ID to delete: ", 1);
            if (assetId == -1) return;

            var asset = await _assetManager.GetAssetByIdAsync(assetId);
            if (asset == null)
            {
                _displayService.DisplayError("Asset not found.");
                _inputService.PauseForUser();
                return;
            }

            Console.WriteLine($"\nAsset to delete:");
            Console.WriteLine($"Type: {asset.AssetType}");
            Console.WriteLine($"Brand: {asset.Brand}");
            Console.WriteLine($"Model: {asset.Model}");
            Console.WriteLine($"Value: {asset.PurchasePrice} (${asset.PurchasePrice.ToUSD():N2} USD)");

            if (_inputService.GetConfirmation("Are you sure you want to delete this asset?"))
            {
                var success = await _assetManager.DeleteAssetAsync(assetId);
                if (success)
                {
                    _displayService.DisplaySuccess("Asset deleted successfully!");
                }
                else
                {
                    _displayService.DisplayError("Failed to delete asset.");
                }
            }
            else
            {
                _displayService.DisplayWarning("Deletion cancelled.");
            }

            _inputService.PauseForUser();
        }

        /// <summary>
        /// Displays various reports about the asset inventory
        /// </summary>
        private static async Task ViewReportsAsync()
        {
            Console.Clear();
            Console.WriteLine("=== Asset Reports ===\n");

            var assets = await _assetManager!.GetAllAssetsAsync();

            // Basic statistics
            Console.WriteLine($"Total Assets: {assets.Count}");

            var computerCount = await _assetManager.GetAssetCountByTypeAsync("computer");
            var phoneCount = await _assetManager.GetAssetCountByTypeAsync("phone");

            Console.WriteLine($"Computers: {computerCount}");
            Console.WriteLine($"Phones: {phoneCount}");

            // Assets by office
            var assetsByOffice = await _assetManager.GetAssetsByOfficeAsync();
            Console.WriteLine("\nAssets by Office:");
            foreach (var office in assetsByOffice.OrderBy(kvp => kvp.Key))
            {
                Console.WriteLine($"  {office.Key}: {office.Value}");
            }

            // End of life analysis
            var nearEndOfLife = assets.Count(a => a.IsNearEndOfLife());
            var approachingEndOfLife = assets.Count(a => a.IsApproachingEndOfLife());

            Console.WriteLine("\nEnd of Life Analysis:");
            Console.WriteLine($"Assets near end of life (< 3 months): {nearEndOfLife}");
            Console.WriteLine($"Assets approaching end of life (3-6 months): {approachingEndOfLife}");

            // Total value calculations
            var totalUsdValue = assets.Sum(a => a.PurchasePrice.ToUSD());
            Console.WriteLine($"\nTotal Portfolio Value: ${totalUsdValue:N2} USD");

            _inputService!.PauseForUser();
        }
    }
}