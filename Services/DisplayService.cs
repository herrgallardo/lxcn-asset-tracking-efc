using lxcn_asset_tracking_efc.Models;

namespace lxcn_asset_tracking_efc.Services
{
    /// <summary>
    /// Service for displaying assets in the console with color coding and formatting
    /// Matches the original implementation's display functionality
    /// </summary>
    public class DisplayService
    {
        /// <summary>
        /// Displays assets sorted by office location and purchase date with color coding
        /// RED: Assets within 3 months of end-of-life
        /// YELLOW: Assets within 6 months of end-of-life
        /// </summary>
        /// <param name="assets">List of assets to display</param>
        public void DisplayAssets(List<Asset> assets)
        {
            if (assets.Count == 0)
            {
                Console.WriteLine("\nNo assets found in inventory.");
                return;
            }

            Console.WriteLine("\nAsset Inventory:");
            Console.WriteLine(new string('-', 160));
            Console.WriteLine($"{"ID",-4} | {"Type",-12} | {"Brand",-12} | {"Model",-25} | {"Office",-10} | {"Purchase Date",-15} | {"Local Price",-15} | {"End of Life",-15}");
            Console.WriteLine(new string('-', 160));

            string previousOffice = "";
            int assetsNearEndOfLife3Months = 0;
            int assetsNearEndOfLife6Months = 0;

            foreach (var asset in assets)
            {
                // Add extra spacing between different office locations
                if (previousOffice != "" && previousOffice != asset.OfficeLocation)
                {
                    Console.WriteLine();
                }
                previousOffice = asset.OfficeLocation;

                // Determine color based on end-of-life status
                if (asset.IsNearEndOfLife())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    assetsNearEndOfLife3Months++;
                }
                else if (asset.IsApproachingEndOfLife())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    assetsNearEndOfLife6Months++;
                }

                // Truncate model name if it's too long to maintain alignment
                var modelDisplay = asset.Model.Length > 25 ? asset.Model.Substring(0, 22) + "..." : asset.Model;

                // Display asset information including ID
                var endOfLifeDate = asset.TimeUntilEndOfLife().Days > 0 ?
                    asset.PurchaseDate.AddYears(3).ToShortDateString() : "EXPIRED";

                Console.WriteLine($"{asset.Id,-4} | {asset.AssetType,-12} | {asset.Brand,-12} | {modelDisplay,-25} | {asset.OfficeLocation,-10} | {asset.PurchaseDate.ToShortDateString(),-15} | {asset.PurchasePrice.ToString(),-15} | {endOfLifeDate,-15}");

                // Reset color
                Console.ResetColor();
            }

            Console.WriteLine(new string('-', 160));
            DisplaySummaryStatistics(assets, assetsNearEndOfLife3Months, assetsNearEndOfLife6Months);
        }

        /// <summary>
        /// Displays summary statistics for the asset inventory
        /// </summary>
        /// <param name="assets">List of all assets</param>
        /// <param name="nearEndOfLife3Months">Count of assets within 3 months of end-of-life</param>
        /// <param name="nearEndOfLife6Months">Count of assets within 6 months of end-of-life</param>
        private void DisplaySummaryStatistics(List<Asset> assets, int nearEndOfLife3Months, int nearEndOfLife6Months)
        {
            Console.WriteLine($"\nSummary Statistics:");
            Console.WriteLine($"Total assets: {assets.Count}");
            Console.WriteLine($"Assets nearing end of life (< 3 months): {nearEndOfLife3Months}");
            Console.WriteLine($"Assets nearing end of life (3-6 months): {nearEndOfLife6Months}");

            var computerCount = assets.Count(a => a.AssetType == "Computer");
            var phoneCount = assets.Count(a => a.AssetType == "Phone");

            Console.WriteLine($"Computers: {computerCount}");
            Console.WriteLine($"Phones: {phoneCount}");

            // Office statistics
            var officeGroups = assets.GroupBy(a => a.OfficeLocation)
                                   .Select(g => new { Office = g.Key, Count = g.Count() });

            Console.WriteLine("\nAssets by Office:");
            foreach (var group in officeGroups.OrderBy(g => g.Office))
            {
                Console.WriteLine($"  {group.Office}: {group.Count}");
            }
        }

        /// <summary>
        /// Displays a menu for user interaction
        /// </summary>
        public void DisplayMainMenu()
        {
            Console.WriteLine("\n=== Asset Tracking System ===");
            Console.WriteLine("1. View All Assets");
            Console.WriteLine("2. Add New Asset");
            Console.WriteLine("3. Update Asset");
            Console.WriteLine("4. Delete Asset");
            Console.WriteLine("5. View Reports");
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect an option (1-6): ");
        }

        /// <summary>
        /// Displays asset type selection menu
        /// </summary>
        public void DisplayAssetTypeMenu()
        {
            Console.WriteLine("\nSelect Asset Type:");
            Console.WriteLine("1. Computer");
            Console.WriteLine("2. Phone");
            Console.Write("Enter choice (1-2): ");
        }

        /// <summary>
        /// Displays currency selection menu
        /// </summary>
        public void DisplayCurrencyMenu()
        {
            Console.WriteLine("\nSelect Currency:");
            Console.WriteLine("1. USD ($)");
            Console.WriteLine("2. EUR (€)");
            Console.WriteLine("3. SEK (kr)");
            Console.Write("Enter choice (1-3): ");
        }

        /// <summary>
        /// Displays office location selection menu
        /// </summary>
        public void DisplayOfficeMenu()
        {
            Console.WriteLine("\nSelect Office Location:");
            Console.WriteLine("1. USA");
            Console.WriteLine("2. Germany");
            Console.WriteLine("3. Sweden");
            Console.Write("Enter choice (1-3): ");
        }

        /// <summary>
        /// Displays error message in red color
        /// </summary>
        /// <param name="message">Error message to display</param>
        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays success message in green color
        /// </summary>
        /// <param name="message">Success message to display</param>
        public void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Success: {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays warning message in yellow color
        /// </summary>
        /// <param name="message">Warning message to display</param>
        public void DisplayWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Warning: {message}");
            Console.ResetColor();
        }
    }
}
