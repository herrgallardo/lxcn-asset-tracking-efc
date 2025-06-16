using System.ComponentModel.DataAnnotations;

namespace lxcn_asset_tracking_efc.Models
{
    /// <summary>
    /// Price class to handle currency and value as in the original implementation
    /// </summary>
    public class Price
    {
        /// <summary>
        /// Primary key for EF Core
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Monetary value of the price
        /// </summary>
        [Required]
        public decimal Value { get; set; }

        /// <summary>
        /// Currency type for this price
        /// </summary>
        [Required]
        public Currency Currency { get; set; }

        /// <summary>
        /// Constructor for creating a price with value and currency
        /// </summary>
        public Price(decimal value, Currency currency)
        {
            Value = value;
            Currency = currency;
        }

        /// <summary>
        /// Parameterless constructor required by EF Core
        /// </summary>
        public Price()
        {
        }

        /// <summary>
        /// Returns formatted price string with currency symbol
        /// </summary>
        public override string ToString()
        {
            string currencySymbol = Currency switch
            {
                Currency.USD => "$",
                Currency.EUR => "€",
                Currency.SEK => "kr",
                _ => ""
            };

            return $"{currencySymbol}{Value:N2}";
        }
    }
}