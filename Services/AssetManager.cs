using Microsoft.EntityFrameworkCore;
using lxcn_asset_tracking_efc.Data;
using lxcn_asset_tracking_efc.Models;

namespace lxcn_asset_tracking_efc.Services
{
    /// <summary>
    /// Service class for managing asset operations including CRUD functionality
    /// Handles both Computer and Phone assets with database persistence
    /// </summary>
    public class AssetManager
    {
        private readonly AssetTrackingContext _context;

        /// <summary>
        /// Constructor that accepts the database context
        /// </summary>
        /// <param name="context">Entity Framework database context</param>
        public AssetManager(AssetTrackingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new asset to the database
        /// </summary>
        /// <param name="asset">Asset to add</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> AddAssetAsync(Asset asset)
        {
            try
            {
                // First add the Price entity
                _context.Prices.Add(asset.PurchasePrice);
                await _context.SaveChangesAsync();

                // Then add the Asset
                _context.Assets.Add(asset);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding asset: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves all assets from the database including their prices
        /// </summary>
        /// <returns>List of all assets</returns>
        public async Task<List<Asset>> GetAllAssetsAsync()
        {
            try
            {
                return await _context.Assets
                    .Include(a => a.PurchasePrice)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving assets: {ex.Message}");
                return new List<Asset>();
            }
        }

        /// <summary>
        /// Retrieves assets sorted by office location and then by purchase date
        /// Matches the original implementation's display requirements
        /// </summary>
        /// <returns>Sorted list of assets</returns>
        public async Task<List<Asset>> GetAssetsSortedAsync()
        {
            try
            {
                return await _context.Assets
                    .Include(a => a.PurchasePrice)
                    .OrderBy(a => a.OfficeLocation)
                    .ThenBy(a => a.PurchaseDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving sorted assets: {ex.Message}");
                return new List<Asset>();
            }
        }

        /// <summary>
        /// Retrieves an asset by its ID
        /// </summary>
        /// <param name="id">Asset ID</param>
        /// <returns>Asset if found, null otherwise</returns>
        public async Task<Asset?> GetAssetByIdAsync(int id)
        {
            try
            {
                return await _context.Assets
                    .Include(a => a.PurchasePrice)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving asset: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Updates an existing asset in the database
        /// </summary>
        /// <param name="asset">Asset with updated information</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> UpdateAssetAsync(Asset asset)
        {
            try
            {
                _context.Assets.Update(asset);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating asset: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes an asset from the database
        /// </summary>
        /// <param name="id">ID of asset to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> DeleteAssetAsync(int id)
        {
            try
            {
                var asset = await GetAssetByIdAsync(id);
                if (asset == null)
                    return false;

                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting asset: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets count of assets by type
        /// </summary>
        /// <param name="assetType">Type of asset (Computer or Phone)</param>
        /// <returns>Count of assets</returns>
        public async Task<int> GetAssetCountByTypeAsync(string assetType)
        {
            try
            {
                return assetType.ToLower() switch
                {
                    "computer" => await _context.Computers.CountAsync(),
                    "phone" => await _context.Phones.CountAsync(),
                    _ => 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting asset count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Gets assets grouped by office location
        /// </summary>
        /// <returns>Dictionary with office as key and asset count as value</returns>
        public async Task<Dictionary<string, int>> GetAssetsByOfficeAsync()
        {
            try
            {
                return await _context.Assets
                    .GroupBy(a => a.OfficeLocation)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting assets by office: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }
    }
}