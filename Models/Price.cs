using System.ComponentModel.DataAnnotations;
using lxcn_asset_tracking_efc.Services;

namespace lxcn_asset_tracking_efc.Models
{
    /// <summary>
    /// Price class to handle currency and value with conversion capabilities
    /// Matches the original GitHub implementation with ToUSD functionality
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
        /// Convert price to USD using CurrencyConverter service
        /// Matches the original GitHub implementation functionality
        /// </summary>
        /// <returns>Price converted to USD</returns>
        public decimal ToUSD()
        {
            if (Currency == Currency.USD)
                return Value;

            string fromCurrencyStr = Currency.ToString();

            if (CurrencyConverter.ConvertTo(Value, fromCurrencyStr, "USD", out decimal convertedValue))
                return convertedValue;

            // Fallback to approximate rates if conversion fails
            decimal fallbackRate = Currency switch
            {
                Currency.EUR => 1.1m,     // Approximate EUR to USD rate
                Currency.SEK => 0.095m,   // Approximate SEK to USD rate
                _ => 1.0m
            };

            return Value * fallbackRate;
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