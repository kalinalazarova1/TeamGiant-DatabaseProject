namespace VehicleVendorConsole.Client
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using VehicleVendor.Data;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Models;

    public class VehicleVendorConsoleClientEntryPoint
    {
        public static void Main()
        {        
            var car = new Vehicle() { Name = "Micra", Price = 12000.00m, Category = Category.Car };
            var car1 = new Vehicle() { Name = "Micra1", Price = 12000.00m, Category = Category.Car };
            var truck = new Vehicle() { Name = "Navara", Price = 25000.00m, Category = Category.Truck };
            var truck1 = new Vehicle() { Name = "Navara1", Price = 25000.00m, Category = Category.Truck };

            var repo = new VehicleVendorRepository(new IVehicleVendorDbContext[] { new VehicleVendorDbContext(), new VehicleVendorMySqlDbContext() });
            repo.Add<Vehicle>(car);
            repo.Add<Vehicle>(truck);
            repo.Add<Vehicle>(car1);
            repo.Add<Vehicle>(truck1);
            repo.SaveChanges();

            var forUpdate = repo.Vehicles.First(v => v.Name == "Micra1");
            var newTruck1 = new Vehicle() { Id = forUpdate.Id, Name = "Pathfinder", Price = 35000.00m, Category = Category.Truck };
            
            var forDelete = repo.Vehicles.First(v => v.Name == "Navara1");
            repo.Delete<Vehicle>(forDelete);
            repo.Update<Vehicle>(forUpdate, newTruck1);

            repo.SaveChanges();

            var result = repo.Vehicles.Select(v => new { name = v.Name, category = v.Category });
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
