using lxcn_asset_tracking_efc.Models;

namespace lxcn_asset_tracking_efc.Services
{
    /// <summary>
    /// Service for creating sample data to populate the database for testing
    /// Creates various computer and phone assets across different offices
    /// </summary>
    public class SampleDataService
    {
        private readonly AssetManager _assetManager;

        /// <summary>
        /// Constructor that accepts the asset manager for database operations
        /// </summary>
        /// <param name="assetManager">Asset manager service</param>
        public SampleDataService(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        /// <summary>
        /// Creates and adds sample assets to the database
        /// Includes computers and phones from different brands and offices
        /// </summary>
        /// <returns>True if sample data was added successfully</returns>
        public async Task<bool> AddSampleDataAsync()
        {
            try
            {
                // Check if we already have data
                var existingAssets = await _assetManager.GetAllAssetsAsync();
                if (existingAssets.Count > 0)
                {
                    Console.WriteLine("Database already contains assets. Skipping sample data creation.");
                    return true;
                }

                Console.WriteLine("Adding sample data...");

                // Sample Computers
                var computers = new List<Computer>
                {
                    new Computer
                    {
                        Brand = "Apple",
                        Model = "MacBook Pro 13",
                        PurchaseDate = new DateTime(2022, 1, 15),
                        PurchasePrice = new Price(1299.99m, Currency.USD),
                        OfficeLocation = "USA"
                    },
                    new Computer
                    {
                        Brand = "Dell",
                        Model = "XPS 15",
                        PurchaseDate = new DateTime(2021, 6, 10),
                        PurchasePrice = new Price(1199.99m, Currency.USD),
                        OfficeLocation = "USA"
                    },
                    new Computer
                    {
                        Brand = "Lenovo",
                        Model = "ThinkPad X1 Carbon",
                        PurchaseDate = new DateTime(2021, 3, 20),
                        PurchasePrice = new Price(1450.00m, Currency.EUR),
                        OfficeLocation = "Germany"
                    },
                    new Computer
                    {
                        Brand = "Asus",
                        Model = "ZenBook 14",
                        PurchaseDate = new DateTime(2022, 8, 5),
                        PurchasePrice = new Price(12500.00m, Currency.SEK),
                        OfficeLocation = "Sweden"
                    },
                    new Computer
                    {
                        Brand = "Apple",
                        Model = "MacBook Air",
                        PurchaseDate = new DateTime(2023, 2, 14),
                        PurchasePrice = new Price(999.99m, Currency.USD),
                        OfficeLocation = "USA"
                    }
                };

                // Sample Phones
                var phones = new List<Phone>
                {
                    new Phone
                    {
                        Brand = "Apple",
                        Model = "iPhone 13 Pro",
                        PurchaseDate = new DateTime(2022, 9, 25),
                        PurchasePrice = new Price(999.99m, Currency.USD),
                        OfficeLocation = "USA"
                    },
                    new Phone
                    {
                        Brand = "Samsung",
                        Model = "Galaxy S22",
                        PurchaseDate = new DateTime(2022, 4, 12),
                        PurchasePrice = new Price(849.99m, Currency.EUR),
                        OfficeLocation = "Germany"
                    },
                    new Phone
                    {
                        Brand = "Nokia",
                        Model = "X20",
                        PurchaseDate = new DateTime(2021, 11, 8),
                        PurchasePrice = new Price(3999.00m, Currency.SEK),
                        OfficeLocation = "Sweden"
                    },
                    new Phone
                    {
                        Brand = "Apple",
                        Model = "iPhone 12",
                        PurchaseDate = new DateTime(2021, 5, 30),
                        PurchasePrice = new Price(699.99m, Currency.USD),
                        OfficeLocation = "USA"
                    },
                    new Phone
                    {
                        Brand = "Samsung",
                        Model = "Galaxy A52",
                        PurchaseDate = new DateTime(2023, 1, 18),
                        PurchasePrice = new Price(399.99m, Currency.EUR),
                        OfficeLocation = "Germany"
                    }
                };

                // Add computers to database
                foreach (var computer in computers)
                {
                    await _assetManager.AddAssetAsync(computer);
                }

                // Add phones to database
                foreach (var phone in phones)
                {
                    await _assetManager.AddAssetAsync(phone);
                }

                Console.WriteLine($"Successfully added {computers.Count} computers and {phones.Count} phones.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding sample data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates assets that are near end-of-life for testing color coding
        /// </summary>
        /// <returns>True if test data was added successfully</returns>
        public async Task<bool> AddEndOfLifeTestDataAsync()
        {
            try
            {
                Console.WriteLine("Adding end-of-life test data...");

                // Asset that is RED (within 3 months of end-of-life)
                var redAsset = new Computer
                {
                    Brand = "Dell",
                    Model = "Latitude 5420 (RED TEST)",
                    PurchaseDate = DateTime.Now.AddYears(-3).AddMonths(1), // 35 months old
                    PurchasePrice = new Price(899.99m, Currency.USD),
                    OfficeLocation = "USA"
                };

                // Asset that is YELLOW (within 6 months of end-of-life)
                var yellowAsset = new Phone
                {
                    Brand = "Nokia",
                    Model = "G50 (YELLOW TEST)",
                    PurchaseDate = DateTime.Now.AddYears(-3).AddMonths(3), // 33 months old
                    PurchasePrice = new Price(2999.00m, Currency.SEK),
                    OfficeLocation = "Sweden"
                };

                await _assetManager.AddAssetAsync(redAsset);
                await _assetManager.AddAssetAsync(yellowAsset);

                Console.WriteLine("Successfully added end-of-life test assets.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding test data: {ex.Message}");
                return false;
            }
        }
    }
}