namespace VehicleVendorConsole.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using VehicleVendor.Data;
    using VehicleVendor.Data.Migrations;
    using VehicleVendor.Models;

    public class VehicleVendorConsoleClinetEntryPoint
    {
        public static void Main(string[] args)
        {
            var context = new VehicleVendorDbContext();
            var data = new VehicleVendorData(context);
            var car = new Vehicle() { Name = "Micra", Price = 12000.00m, Category = Category.Car};
            var truck = new Vehicle() { Name = "Navara", Price = 25000.00m, Category = Category.Truck };
            data.Vehicles.Add(car);
            data.Vehicles.Add(truck);
            data.SaveChanges();
            var result = data.Vehicles.All().Select(v => new { name = v.Name, category = v.Category});
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
