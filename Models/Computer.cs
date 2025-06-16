namespace lxcn_asset_tracking_efc.Models
{
    /// <summary>
    /// Computer class that inherits from Asset - represents laptops and desktop computers
    /// Supports MacBook, Asus, Lenovo and other brands
    /// </summary>
    public class Computer : Asset
    {
        /// <summary>
        /// Override of the AssetType property from the base class
        /// </summary>
        public override string AssetType => "Computer";
    }
}