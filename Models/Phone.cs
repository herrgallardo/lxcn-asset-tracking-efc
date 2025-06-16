namespace lxcn_asset_tracking_efc.Models
{
    /// <summary>
    /// Phone class that inherits from Asset - represents mobile phones
    /// Supports iPhone, Samsung, Nokia and other brands
    /// </summary>
    public class Phone : Asset
    {
        /// <summary>
        /// Override of the AssetType property from the base class
        /// </summary>
        public override string AssetType => "Phone";
    }
}