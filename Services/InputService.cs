using lxcn_asset_tracking_efc.Models;

namespace lxcn_asset_tracking_efc.Services
{
    /// <summary>
    /// Service for handling user input and validation
    /// Provides methods to collect asset information from console input
    /// </summary>
    public class InputService
    {
        private readonly DisplayService _displayService;

        /// <summary>
        /// Constructor that accepts display service for showing prompts
        /// </summary>
        /// <param name="displayService">Service for console display operations</param>
        public InputService(DisplayService displayService)
        {
            _displayService = displayService;
        }

        /// <summary>
        /// Prompts user to create a new asset (Computer or Phone)
        /// </summary>
        /// <returns>New asset instance or null if cancelled</returns>
        public Asset? CreateAsset()
        {
            _displayService.DisplayAssetTypeMenu();

            if (!int.TryParse(Console.ReadLine(), out int assetType) || assetType < 1 || assetType > 2)
            {
                _displayService.DisplayError("Invalid asset type selection.");
                return null;
            }

            return assetType switch
            {
                1 => CreateComputer(),
                2 => CreatePhone(),
                _ => null
            };
        }

        /// <summary>
        /// Creates a new Computer asset with user input
        /// </summary>
        /// <returns>Computer instance or null if cancelled</returns>
        private Computer? CreateComputer()
        {
            try
            {
                Console.WriteLine("\n=== Creating New Computer ===");

                var brand = GetStringInput("Enter brand (e.g., Apple, Dell, Lenovo, Asus): ");
                if (string.IsNullOrEmpty(brand)) return null;

                var model = GetStringInput("Enter model name: ");
                if (string.IsNullOrEmpty(model)) return null;

                var purchaseDate = GetDateInput("Enter purchase date (yyyy-mm-dd): ");
                if (purchaseDate == DateTime.MinValue) return null;

                var price = GetPriceInput();
                if (price == null) return null;

                var officeLocation = GetOfficeLocation();
                if (string.IsNullOrEmpty(officeLocation)) return null;

                return new Computer
                {
                    Brand = brand,
                    Model = model,
                    PurchaseDate = purchaseDate,
                    PurchasePrice = price,
                    OfficeLocation = officeLocation
                };
            }
            catch (Exception ex)
            {
                _displayService.DisplayError($"Error creating computer: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a new Phone asset with user input
        /// </summary>
        /// <returns>Phone instance or null if cancelled</returns>
        private Phone? CreatePhone()
        {
            try
            {
                Console.WriteLine("\n=== Creating New Phone ===");

                var brand = GetStringInput("Enter brand (e.g., Apple, Samsung, Nokia): ");
                if (string.IsNullOrEmpty(brand)) return null;

                var model = GetStringInput("Enter model name: ");
                if (string.IsNullOrEmpty(model)) return null;

                var purchaseDate = GetDateInput("Enter purchase date (yyyy-mm-dd): ");
                if (purchaseDate == DateTime.MinValue) return null;

                var price = GetPriceInput();
                if (price == null) return null;

                var officeLocation = GetOfficeLocation();
                if (string.IsNullOrEmpty(officeLocation)) return null;

                return new Phone
                {
                    Brand = brand,
                    Model = model,
                    PurchaseDate = purchaseDate,
                    PurchasePrice = price,
                    OfficeLocation = officeLocation
                };
            }
            catch (Exception ex)
            {
                _displayService.DisplayError($"Error creating phone: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets string input from user with validation
        /// </summary>
        /// <param name="prompt">Prompt message to display</param>
        /// <returns>User input string or empty if invalid</returns>
        private string GetStringInput(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                _displayService.DisplayError("Input cannot be empty.");
                return string.Empty;
            }

            return input;
        }

        /// <summary>
        /// Gets date input from user with validation
        /// </summary>
        /// <param name="prompt">Prompt message to display</param>
        /// <returns>Valid DateTime or DateTime.MinValue if invalid</returns>
        private DateTime GetDateInput(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();

            if (DateTime.TryParse(input, out DateTime date))
            {
                if (date <= DateTime.Now.AddYears(-10) || date > DateTime.Now)
                {
                    _displayService.DisplayError("Date must be between 10 years ago and today.");
                    return DateTime.MinValue;
                }
                return date;
            }

            _displayService.DisplayError("Invalid date format. Please use yyyy-mm-dd.");
            return DateTime.MinValue;
        }

        /// <summary>
        /// Gets price input with currency selection
        /// </summary>
        /// <returns>Price instance or null if invalid</returns>
        private Price? GetPriceInput()
        {
            Console.Write("Enter price amount: ");
            var priceInput = Console.ReadLine()?.Trim();

            if (!decimal.TryParse(priceInput, out decimal amount) || amount <= 0)
            {
                _displayService.DisplayError("Invalid price amount.");
                return null;
            }

            _displayService.DisplayCurrencyMenu();
            if (!int.TryParse(Console.ReadLine(), out int currencyChoice) || currencyChoice < 1 || currencyChoice > 3)
            {
                _displayService.DisplayError("Invalid currency selection.");
                return null;
            }

            var currency = currencyChoice switch
            {
                1 => Currency.USD,
                2 => Currency.EUR,
                3 => Currency.SEK,
                _ => Currency.USD
            };

            return new Price(amount, currency);
        }

        /// <summary>
        /// Gets office location selection from user
        /// </summary>
        /// <returns>Office location string or empty if invalid</returns>
        private string GetOfficeLocation()
        {
            _displayService.DisplayOfficeMenu();
            if (!int.TryParse(Console.ReadLine(), out int officeChoice) || officeChoice < 1 || officeChoice > 3)
            {
                _displayService.DisplayError("Invalid office selection.");
                return string.Empty;
            }

            return officeChoice switch
            {
                1 => "USA",
                2 => "Germany",
                3 => "Sweden",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Gets integer input from user with validation
        /// </summary>
        /// <param name="prompt">Prompt message to display</param>
        /// <param name="min">Minimum allowed value</param>
        /// <param name="max">Maximum allowed value</param>
        /// <returns>Valid integer or -1 if invalid</returns>
        public int GetIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out int result))
            {
                if (result >= min && result <= max)
                {
                    return result;
                }
                _displayService.DisplayError($"Value must be between {min} and {max}.");
            }
            else
            {
                _displayService.DisplayError("Invalid number format.");
            }

            return -1;
        }

        /// <summary>
        /// Prompts user to continue or press any key
        /// </summary>
        /// <param name="message">Message to display</param>
        public void PauseForUser(string message = "Press any key to continue...")
        {
            Console.WriteLine($"\n{message}");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Gets confirmation from user (y/n)
        /// </summary>
        /// <param name="prompt">Confirmation prompt</param>
        /// <returns>True if user confirms, false otherwise</returns>
        public bool GetConfirmation(string prompt)
        {
            Console.Write($"{prompt} (y/n): ");
            var input = Console.ReadLine()?.Trim().ToLower();
            return input == "y" || input == "yes";
        }
    }
}