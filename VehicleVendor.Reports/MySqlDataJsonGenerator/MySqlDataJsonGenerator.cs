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
        private IVehicleVendorMySqlRepository mySqlRepository;

        public MySqlDataJsonGenerator(IVehicleVendorRepository vehicleVendorRepository, IVehicleVendorMySqlRepository mySqlRepository)
        {
            this.repository = vehicleVendorRepository;
            this.mySqlRepository = mySqlRepository;
        }

        public void WriteJsonsReportsToMySql()
        {
            List<JsonReportModel> jsonReports = this.ParseAllJsonReports();

            foreach (var model in jsonReports)
            {
                var income = new Income()
                {
                    DealerId = model.DealerId,
                    Date = model.Date,
                    Amount = model.Amount
                };

                this.mySqlRepository.Add<Income>(income);
                this.mySqlRepository.SaveChanges();
            }
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
            var reportModel = JsonConvert.DeserializeObject<JsonReportModel>(json);

            return reportModel;
        }

        private IList<DateTime> GetIncomeDates()
        {
            var collection = this.repository.Sales
                .Select(x => x.SaleDate)
                .GroupBy(x => x)
                .Select(x => x.Key)
                .ToList();

            return collection;
        }

        private List<JsonReportModel> ParseAllJsonReports()
        {
            var dates = this.GetIncomeDates();
            string currJsonString = string.Empty;
            List<JsonReportModel> jsonModels = new List<JsonReportModel>();

            foreach (var date in dates)
            {
                currJsonString = this.ReadJsonReport(@"../../Reports/" + date.ToShortDateString() + ".json");
                var currJsonModel = this.ConvertJsonReports(currJsonString);
                jsonModels.Add(currJsonModel);
            }

            return jsonModels;
        }
    }
}
