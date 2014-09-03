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
    using VehicleVendor.Reports.XmlReportSqlServerGenerator;
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
                });
            var repoMySql = new VehicleVendorMySqlRepository(new VehicleVendorMySqlDbContext());
            var nissanMongoDb = new VehicleVendorMongoDb();

            var mongoLoader = new MongoLoader(repo, nissanMongoDb);
            mongoLoader.LoadRepository();
            repo.SaveChanges();

            var xmlParser = new XmlParser(repo);
            var parseResult = xmlParser.ParseDiscounts(@"..\..\..\Discounts.xml", @"..\..\..\Discounts.xsd");
            var xmlLoader = new XmlLoader(repo, parseResult);
            xmlLoader.LoadRepository();
            repo.SaveChanges();


            var excelReporter = new ExcelReportsSQLiteGenerator(repoMySql, new DateTime(2014, 8, 1), new DateTime(2014, 9, 1));
            excelReporter.GenerateReport();
            
            var pdfReporter = new PdfReportSQLServerGenerator(repo);
            pdfReporter.GenerateReport();
            
            var jsonReporter = new JsonReportSQLServerGenerator(repo);
            jsonReporter.GenerateReport();
            
            var jsonToMySql = new MySqlDataJsonLoader(repo, repoMySql);
            jsonToMySql.WriteJsonsReportsToMySql(); 
        }
    }
}