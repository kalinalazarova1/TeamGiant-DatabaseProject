using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleVendor.Data;
using VehicleVendor.Data.Repositories;
using VehicleVendor.Reports.JsonReportSQLServerGenerator;
using VehicleVendor.Models;

namespace VehicleVendor.Reports.MySqlDataJsonGenerator
{
    public class MySqlDataJsonGenerator
    {
        private IVehicleVendorRepository repository;
        private IVehicleVendorRepository mySqlRepository;

        public MySqlDataJsonGenerator(IVehicleVendorRepository vehicleVendorRepository)
        {
            this.repository = vehicleVendorRepository;
            mySqlRepository = new VehicleVendorRepository(
                new IVehicleVendorDbContext[]
                { 
                    new VehicleVendorDbContextInMySql()
                });
        }

        public void WriteJsonsReportsToMySql()
        {
            List<JsonReportModel> jsonReports = this.ParseAllJsonReports();

            foreach (var model in jsonReports)
            {
                var vehicle = new Vehicle()
                {
                    //Id = model.ProductId,
                    Name = model.Model,
                    Category = model.Category,
                    Price = model.Price
                };

                this.mySqlRepository.Add<Vehicle>(vehicle);
            }

            this.mySqlRepository.SaveChanges();
        }

        private string ReadJsonReport(string path)
        {
            string json = string.Empty;

            using (StreamReader reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }

            return json;
        }

        private JsonReportModel ConvertJsonReports(string json)
        {
            JsonReportModel reportModel = JsonConvert.DeserializeObject<JsonReportModel>(json);

            return reportModel;
        }

        private IList<int> GetProductIds()
        {
            var collection = this.repository.Vehicles
                .Select(x => x.Id)
                .ToList();

            return collection;
        }

        private List<JsonReportModel> ParseAllJsonReports()
        {
            var ids = this.GetProductIds();
            string currJsonString = string.Empty;
            List<JsonReportModel> jsonModels = new List<JsonReportModel>();

            foreach (var id in ids)
            {
                currJsonString = this.ReadJsonReport(@"../../Reports/" + id + ".json");
                JsonReportModel currJsonModel = this.ConvertJsonReports(currJsonString);
                jsonModels.Add(currJsonModel);
            }

            return jsonModels;
        }
    }
}
