namespace VehicleVendorConsole.Client
{
    using System;
    using System.Linq;
    using VehicleVendor.Data;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Models;
    using VehicleVendor.Reports;
    using PdfReportCreator;
    using VehicleVendorConsole.Client.XmlInput;
    using VehicleVendor.Reports.JsonReportSQLServerGenerator;
    using VehicleVendor.Reports.MySqlDataJsonGenerator;

    public class VehicleVendorConsoleClientEntryPoint
    {
        public static void Main()
        {
            var repo = new VehicleVendorRepository(
                new IVehicleVendorDbContext[]
                { 
                    new VehicleVendorDbContext()
                    //new VehicleVendorDbContextInMySql() 
                });
            var repoMySql = new VehicleVendorMySqlRepository(new VehicleVendorMySqlDbContext());
            repoMySql.Add<Country>(new Country());
            repoMySql.SaveChanges();
            var nissanMongoDb = new VehicleVendorMongoDb();
            var mongoLoader = new MongoLoader(repo, nissanMongoDb);
            mongoLoader.LoadRepository();
            repo.SaveChanges();

            var xmlimporter = new XmlImporter(repo);
            var parseResult = xmlimporter.ParseDiscounts(@"..\..\..\Discounts.xml", @"..\..\..\Discounts.xsd");
            mongoLoader.LoadDiscounts(parseResult);
            repo.SaveChanges();

            mongoLoader.LoadDiscountsInMongo(parseResult);

            // sample add sale:
            // ============================================================
            //var sale = new Sale() { DealerId = 1, SaleDate = new DateTime(2014, 8, 31) };
            //repo.Add<Sale>(sale);
            //var details = new SaleDetails() { Quantity = 1000, VehicleId = 1, Sale = sale };
            //repo.Add<SaleDetails>(details);
            //details = new SaleDetails() { Quantity = 1000, VehicleId = 2, Sale = sale };
            //repo.Add<SaleDetails>(details);
            //repo.SaveChanges();


            var zipExLoader = new ZipExcelLoader(repo);
            zipExLoader.LoadRepository();
            repo.SaveChanges();

            repoMySql.Add<Dealer>(new Dealer());
            repoMySql.SaveChanges();

            // =============================================================

            // sample generate report Excel:
            // =============================================================
            var excelReporter = new ExcelReportsSQLiteGenerator(repo, new DateTime(2014, 8, 1), new DateTime(2014, 9, 1));
            excelReporter.GenerateReport();
            // =============================================================

            // =============================================================
            // Pdf file isngenerated in the main folder
            // Example usage of the PDF Report Generator:
            GeneratePDF pdf = new GeneratePDF(repo);
            pdf.Report("../../PdfReport.pdf");
            // =============================================================

            // =============================================================
            // Example usage of the JSON Report Generator:
            JsonReportSQLServerGenerator json = new JsonReportSQLServerGenerator(repo);
            json.GenerateReport();
            // =============================================================

            // =============================================================
            // Example usage of importing Jsons in MySql:
            // First generate JSON reports
            MySqlDataJsonGenerator jsonToMySql = new MySqlDataJsonGenerator(repo, repoMySql);
            jsonToMySql.WriteJsonsReportsToMySql();
            // =============================================================

            // Example usage of the repository:

            //var car = new Vehicle() { Name = "Micra", Price = 12000.00m, Category = Category.Car };
            //var car1 = new Vehicle() { Name = "Micra1", Price = 12000.00m, Category = Category.Car };
            //var truck = new Vehicle() { Name = "Navara", Price = 25000.00m, Category = Category.Truck };
            //var truck1 = new Vehicle() { Name = "Navara1", Price = 25000.00m, Category = Category.Truck };

            //var repo = new VehicleVendorRepository(new IVehicleVendorDbContext[] 
            //{ 
            //    new VehicleVendorDbContext()
            //    //new VehicleVendorMySqlDbContext()
            //});

            //repo.Add<Vehicle>(car);
            //repo.Add<Vehicle>(truck);
            //repo.Add<Vehicle>(car1);
            //repo.Add<Vehicle>(truck1);
            //repo.SaveChanges();

            //var forUpdate = repo.Vehicles.First(v => v.Name == "Micra1");
            //var newTruck1 = new Vehicle() { Id = forUpdate.Id, Name = "Pathfinder", Price = 35000.00m, Category = Category.Truck };

            //var forDelete = repo.Vehicles.First(v => v.Name == "Navara1");
            //repo.Delete<Vehicle>(forDelete);
            //repo.Update<Vehicle>(forUpdate, newTruck1);

            //repo.SaveChanges();

            //var result = repo.Vehicles.Select(v => new { name = v.Name, category = v.Category });
            //foreach (var item in result)
            //{
            //    Console.WriteLine(item);
            //}
             
        }
    }
}