using System.Xml;
using System.Xml.Serialization;

namespace lxcn_asset_tracking_efc.Services
{
    /// <summary>
    /// Currency conversion service that fetches live exchange rates from European Central Bank
    /// Matches the original GitHub implementation functionality
    /// </summary>
    public static class CurrencyConverter
    {
        private static string xmlUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        private static Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>();
        private static DateTime lastUpdated = DateTime.MinValue;

        /// <summary>
        /// Updates exchange rates from ECB with error suppression option
        /// </summary>
        /// <param name="suppressErrors">If true, doesn't display error messages</param>
        /// <returns>True if rates were updated successfully</returns>
        public static bool Update(bool suppressErrors = false)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                var xmlContent = client.GetStringAsync(xmlUrl).Result;

                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                var namespaceManager = new XmlNamespaceManager(doc.NameTable);
                namespaceManager.AddNamespace("gesmes", "http://www.gesmes.org/xml/ns");
                namespaceManager.AddNamespace("eurofxref", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");

                var cubeNodes = doc.SelectNodes("//eurofxref:Cube[@currency]", namespaceManager);

                exchangeRates.Clear();
                exchangeRates["EUR"] = 1.0m; // EUR is the base currency

                if (cubeNodes != null)
                {
                    foreach (XmlNode node in cubeNodes)
                    {
                        var currency = node.Attributes?["currency"]?.Value;
                        var rateStr = node.Attributes?["rate"]?.Value;

                        if (!string.IsNullOrEmpty(currency) && !string.IsNullOrEmpty(rateStr) &&
                            decimal.TryParse(rateStr, System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out decimal rate))
                        {
                            exchangeRates[currency] = rate;
                        }
                    }
                }

                lastUpdated = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                if (!suppressErrors)
                {
                    Console.WriteLine($"Error updating currency rates: {ex.Message}");
                    Console.WriteLine("Using fallback exchange rates.");
                }

                // Fallback rates if ECB is unavailable
                SetFallbackRates();
                return false;
            }
        }

        /// <summary>
        /// Sets fallback exchange rates when ECB is unavailable
        /// </summary>
        private static void SetFallbackRates()
        {
            exchangeRates.Clear();
            exchangeRates["EUR"] = 1.0m;
            exchangeRates["USD"] = 1.1m;    // Approximate EUR to USD
            exchangeRates["SEK"] = 10.5m;   // Approximate EUR to SEK
            lastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Converts an amount from one currency to another
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromCurrency">Source currency code</param>
        /// <param name="toCurrency">Target currency code</param>
        /// <param name="result">Converted amount</param>
        /// <returns>True if conversion successful</returns>
        public static bool ConvertTo(decimal amount, string fromCurrency, string toCurrency, out decimal result)
        {
            result = 0;

            // Ensure we have rates
            if (exchangeRates.Count == 0 || DateTime.Now.Subtract(lastUpdated).TotalHours > 24)
            {
                Update(true);
            }

            // Same currency
            if (fromCurrency == toCurrency)
            {
                result = amount;
                return true;
            }

            try
            {
                // Convert to EUR first
                decimal amountInEur = amount;
                if (fromCurrency != "EUR")
                {
                    if (!exchangeRates.ContainsKey(fromCurrency))
                        return false;
                    amountInEur = amount / exchangeRates[fromCurrency];
                }

                // Convert from EUR to target currency
                if (toCurrency == "EUR")
                {
                    result = amountInEur;
                }
                else
                {
                    if (!exchangeRates.ContainsKey(toCurrency))
                        return false;
                    result = amountInEur * exchangeRates[toCurrency];
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if valid exchange rates are available
        /// </summary>
        /// <returns>True if rates are available</returns>
        public static bool HasValidRates()
        {
            return exchangeRates.Count > 0 &&
                   exchangeRates.ContainsKey("USD") &&
                   exchangeRates.ContainsKey("SEK");
        }

        /// <summary>
        /// Gets the last update time for exchange rates
        /// </summary>
        /// <returns>DateTime when rates were last updated</returns>
        public static DateTime GetLastUpdateTime()
        {
            return lastUpdated;
        }
    }
}