namespace VehicleVendor.Data
{
    using System.Data.Entity;
    using VehicleVendor.Data.Migrations;
    using VehicleVendor.Models;

    public class VehicleVendorMySqlDbContext : DbContext, IVehicleVendorDbContext
    {
        public VehicleVendorMySqlDbContext()
            : base("Nissan")
        {
            Database.SetInitializer<VehicleVendorMySqlDbContext>(
                new MigrateDatabaseToLatestVersion<VehicleVendorMySqlDbContext, ConfigurationMySql>(
                    "Nissan"));
        }

        public DbSet<Dealer> Dealers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleDetails> SalesDetails { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
