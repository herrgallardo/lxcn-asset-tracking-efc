using Microsoft.EntityFrameworkCore;
using lxcn_asset_tracking_efc.Models;
using lxcn_asset_tracking_efc.Services;

namespace lxcn_asset_tracking_efc.Data
{
    /// <summary>
    /// Entity Framework database context for the Asset Tracking application
    /// Manages database operations for assets, computers, and phones
    /// </summary>
    public class AssetTrackingContext : DbContext
    {
        /// <summary>
        /// Constructor that accepts DbContext options for dependency injection
        /// </summary>
        /// <param name="options">Database context configuration options</param>
        public AssetTrackingContext(DbContextOptions<AssetTrackingContext> options) : base(options)
        {
        }

        /// <summary>
        /// Parameterless constructor for design-time operations (migrations)
        /// </summary>
        public AssetTrackingContext()
        {
        }

        /// <summary>
        /// DbSet for managing Asset entities (includes both Computers and Phones)
        /// </summary>
        public DbSet<Asset> Assets { get; set; }

        /// <summary>
        /// DbSet for managing Computer entities specifically
        /// </summary>
        public DbSet<Computer> Computers { get; set; }

        /// <summary>
        /// DbSet for managing Phone entities specifically
        /// </summary>
        public DbSet<Phone> Phones { get; set; }

        /// <summary>
        /// DbSet for managing Price entities
        /// </summary>
        public DbSet<Price> Prices { get; set; }

        /// <summary>
        /// Configures the database connection for design-time operations
        /// </summary>
        /// <param name="optionsBuilder">Options builder for configuring the context</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    var configService = new ConfigurationService();
                    var connectionString = configService.GetConnectionString();
                    optionsBuilder.UseSqlServer(connectionString);
                }
                catch
                {
                    // Fallback for design-time when appsettings.json might not be available
                    optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AssetTracking;Integrated Security=True");
                }
            }
        }

        /// <summary>
        /// Configures the model relationships and database schema
        /// </summary>
        /// <param name="modelBuilder">The model builder instance</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Asset hierarchy using Table-Per-Hierarchy (TPH) strategy
            // EF Core will automatically create a discriminator column called "Discriminator"
            modelBuilder.Entity<Asset>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Computer>("Computer")
                .HasValue<Phone>("Phone");

            // Configure decimal precision for prices to handle currency values properly
            modelBuilder.Entity<Price>()
                .Property(p => p.Value)
                .HasPrecision(18, 2);

            // Configure the relationship between Asset and Price
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.PurchasePrice)
                .WithMany()
                .HasForeignKey("PurchasePriceId")
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Currency enum to be stored as string
            modelBuilder.Entity<Price>()
                .Property(p => p.Currency)
                .HasConversion<string>();
        }
    }
}