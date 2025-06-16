# Asset Tracking Application

## Course Context

- **Program**: Arbetsmarknadsutbildning - IT Påbyggnad/Programmerare
- **Course**: C# .NET Fullstack System Developer
- **Minitask**: Weekly Assignment #2 - Asset Tracking Application with Entity Framework Core

## Learning Objectives

This console application demonstrates advanced C# programming concepts:

- Object-Oriented Programming with inheritance and polymorphism
- Console Application Development with color-coded output
- Entity Framework Core and Database Integration
- Currency Conversion with live exchange rates
- Date-based Asset Management
- Multi-office inventory tracking
- Full CRUD operations

## Overview

A console-based asset management system built to help companies track their electronic assets such as computers and phones across multiple office locations. The application handles different currencies, calculates asset end-of-life timing, provides visual indicators for assets approaching replacement dates, and stores all data persistently in an Azure SQL Database.

## Features

- Track multiple types of assets (Computers, Phones)
- Sort assets by office location and purchase date
- Color-coded alerts for assets nearing end-of-life
- Multi-currency support with automatic conversion
- Azure SQL Database integration for cloud-based data persistence
- Store asset data with Entity Framework Core
- Full CRUD operations (Create, Read, Update, Delete)
- Interactive menu-driven interface
- Display comprehensive inventory reports
- Live currency exchange rates from European Central Bank

## Key Components

### Classes

- `Asset`: Abstract base class for all trackable items
- `Computer`, `Phone`: Asset subclasses for specific device types
- `Price`: Handles multi-currency support with USD conversion
- `AssetManager`: Asset collection management and database operations
- `CurrencyConverter`: Handles exchange rate updates and conversions
- `DisplayService`: Console formatting and color-coded output
- `InputService`: User input validation and collection

### Services

- `AssetTrackingContext`: Entity Framework database context
- `ConfigurationService`: Application configuration management
- `SampleDataService`: Test data generation for development

## Technical Skills Demonstrated

- C# inheritance and polymorphism
- Entity Framework Core with Table-Per-Hierarchy inheritance
- Azure SQL Database integration
- Web service integration (ECB currency rates)
- LINQ queries for data manipulation
- Color-coded console output
- Date calculations and comparisons
- Async/await programming patterns
- Service-based architecture design

## Application Flow

1. Initialize database connection and Entity Framework context
2. Initialize currency converter with live exchange rates
3. Check database contents and offer sample data if empty
4. Present interactive menu system with:
   - Asset viewing with office location grouping
   - Purchase date sorting within groups
   - Color highlighting for assets approaching end-of-life
   - Full CRUD operations for asset management
5. Display comprehensive statistics and reports

## Requirements

- .NET SDK 9.0 or later
- Azure account (free tier available)
- Internet connection (for currency rate updates)
- Visual Studio Code or similar IDE

## How to Run

### Setup

1. Create Azure SQL Database (free tier)
2. Create `appsettings.json` file with your Azure connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=AssetTrackingDB;User ID=assetadmin;Password=your-password;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

3. Update the connection string with your actual Azure credentials
4. Update the connection string with your actual Azure credentials
5. Install dependencies:

```bash
dotnet restore
```

5. Apply database migrations:

```bash
dotnet ef database update
```

6. Run the application:

```bash
dotnet run
```

## Implementation Details

### Asset End-of-Life Indicators

- **RED**: Less than 3 months until 3-year end-of-life date
- **YELLOW**: Less than 6 months until 3-year end-of-life date

### Office Locations

- USA (USD)
- Sweden (SEK)
- Germany (EUR)

### Supported Currency Conversion

The application uses real-time currency rates from the European Central Bank with fallback rates:

- USD to EUR conversion
- SEK to EUR conversion
- USD to SEK conversion

All prices are displayed in both local currency and USD equivalent.

### Database Schema

Uses Entity Framework Core with Table-Per-Hierarchy inheritance:

```sql
-- Assets table (includes discriminator for Computer/Phone)
Assets (Id, Brand, Model, PurchaseDate, OfficeLocation, PurchasePriceId, Discriminator)

-- Prices table
Prices (Id, Value, Currency)
```

## Learning Notes

This application builds on previous console application skills while adding significant complexity through Entity Framework Core, database persistence, and interactive user interfaces. The combination of inheritance, currency management, live data fetching, and CRUD operations provides a comprehensive learning experience in modern C# development practices.

## Educational Program Details

- **Type**: Arbetsmarknadsutbildning (Labor Market Training)
- **Focus**: IT Påbyggnad/Programmerare (IT Advanced/Programmer)
- **Course**: C# .NET Fullstack System Developer

## Potential Improvements for Future Learning

- Implement web interface with ASP.NET Core
- Add user authentication and authorization
- Create REST API endpoints
- Add asset image upload functionality
- Generate reports exportable to PDF or Excel
- Implement email notifications for end-of-life alerts
- Add asset depreciation calculations
- Create mobile app interface
- Implement asset barcode scanning
- Add asset maintenance tracking
