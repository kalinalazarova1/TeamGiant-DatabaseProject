namespace VehicleVendor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using VehicleVendor.Data.Migrations;
    using VehicleVendor.Models;

    public class VendorContext : DbContext
    {
        public VendorContext()
            : base("Nissan")
        {
            Database.SetInitializer<VendorContext>(new MigrateDatabaseToLatestVersion<VendorContext, Configuration>());
        }

        public DbSet<Dealer> Dealers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleDetails> SalesDetils { get; set; }
    }
}
