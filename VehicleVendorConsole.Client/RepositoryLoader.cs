namespace VehicleVendorConsole.Client
{
    using System.Linq;
    using VehicleVendor.Data;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Models;

    public class RepositoryLoader : IRepositoryLoader
    {
        private IVehicleVendorRepository repo;
        private IVehicleVendorMongoDb nissanMongoDb;

        public RepositoryLoader(IVehicleVendorRepository repo, IVehicleVendorMongoDb nissanMongoDb)
        {
            this.repo = repo;
            this.nissanMongoDb = nissanMongoDb;
        }

        public void LoadRepository()
        {
            this.LoadCountries();
            this.LoadDealers();
            this.LoadVehicles();
        }

        private void LoadVehicles()
        {
            if (this.repo.Vehicles.Count() == 0)
            {
                var vehicles = this.nissanMongoDb.GetDocument("Vehicles");
                foreach (var item in vehicles)
                {
                    this.repo.Add<Vehicle>(
                        new Vehicle()
                        {
                            Name = item["name"].ToString(),
                            Price = (decimal)item["price"].ToDouble(),
                            Category = (Category)item["category"].ToInt32()
                        });
                }
            }
        }

        private void LoadCountries()
        {
            if (this.repo.Countries.Count() == 0)
            {
                var coutries = this.nissanMongoDb.GetDocument("Countries");
                foreach (var item in coutries)
                {
                    this.repo.Add<Country>(
                        new Country()
                        {
                            Name = item["Country"].ToString(),
                            Region = (Region)item["Region"].ToInt32()
                        });
                }
            }
        }

        private void LoadDealers()
        {
            if (this.repo.Dealers.Count() == 0)
            {
                var dealers = this.nissanMongoDb.GetDocument("Dealers");
                foreach (var item in dealers)
                {
                    var countryName = item["country"].ToString();
                    var country = this.repo.Countries.Local.First(c => c.Name == countryName);
                    this.repo.Add<Dealer>(
                        new Dealer()
                        {
                            Company = item["company"].ToString(),
                            Address = item["address"].ToString(),
                            Country = country
                        });
                }
            }
        }
    }
}
