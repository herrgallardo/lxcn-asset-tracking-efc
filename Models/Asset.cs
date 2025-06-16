using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lxcn_asset_tracking_efc.Models
{
    /// <summary>
    /// Abstract base class for all assets matching the original implementation
    /// All assets have a 3-year end of life as specified
    /// </summary>
    public abstract class Asset
    {
        /// <summary>
        /// Primary key for the asset
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Brand/manufacturer of the asset
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Specific model name of the asset
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Date when the asset was purchased
        /// </summary>
        [Required]
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Purchase price with currency information
        /// </summary>
        [Required]
        public virtual Price PurchasePrice { get; set; } = new Price();

        /// <summary>
        /// Office location where the asset is deployed
        /// </summary>
        [Required]
        [StringLength(50)]
        public string OfficeLocation { get; set; } = string.Empty;

        /// <summary>
        /// Abstract property to be implemented by derived classes
        /// This property is not mapped to database - used only for business logic
        /// </summary>
        [NotMapped]
        public abstract string AssetType { get; }

        /// <summary>
        /// Calculates time until end of life (3 years from purchase)
        /// </summary>
        public TimeSpan TimeUntilEndOfLife()
        {
            DateTime endOfLifeDate = PurchaseDate.AddYears(3);
            return endOfLifeDate - DateTime.Now;
        }

        /// <summary>
        /// Determines if asset is within 3 months of end-of-life (RED status)
        /// </summary>
        public bool IsNearEndOfLife()
        {
            var timeUntil = TimeUntilEndOfLife();
            return timeUntil.TotalDays < 90 && timeUntil.TotalDays > 0;
        }

        /// <summary>
        /// Determines if asset is within 6 months of end-of-life (YELLOW status)
        /// </summary>
        public bool IsApproachingEndOfLife()
        {
            var timeUntil = TimeUntilEndOfLife();
            return timeUntil.TotalDays < 180 && timeUntil.TotalDays > 90;
        }
    }
}