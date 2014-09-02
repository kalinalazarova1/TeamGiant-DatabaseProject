using System.Collections.Generic;
using VehicleVendor.Data;
using VehicleVendor.Data.Repositories;
using VehicleVendor.Models;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
namespace VehicleVendor.Reports.JsonReportSQLServerGenerator
{
    public class JsonReportSQLServerGenerator : IReportGenerator
    {
        private IVehicleVendorRepository repository;

        public JsonReportSQLServerGenerator(IVehicleVendorRepository vehicleVendorRepository)
        {
            this.repository = vehicleVendorRepository;
        }

        public void GenerateReport()
        {
            var reports = this.GetReportData();
            this.WriteToFileSystem(@"../../Reports/", reports);
        }

        private void WriteToFileSystem(string path, IEnumerable<JsonReportModel> reportCollection)
        {
            foreach (var rep in reportCollection)
            {
                var data = new JsonReportModel
                {
                    Date = rep.Date,
                    DealerId = rep.DealerId,
                    Amount = rep.Amount
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                using (StreamWriter sw = new StreamWriter(path + rep.Date.ToShortDateString() + ".json"))
                {
                    sw.WriteLine(json);
                }
            }
        }

        private List<JsonReportModel> GetReportData()
        {
            var collection = this.repository.Sales
                .Join(this.repository.SalesDetails, h => h.Id, d => d.SaleId, (h, d) => new { h = h, d = d })
                .Join(this.repository.Dealers, s => s.h.DealerId, d => d.Id, (s, d) => new { s = s, d = d })
                .Join(this.repository.Countries, a => a.d.CountryId, c => c.Id, (a, c) => new { a = a, c = c })
                .Join(this.repository.Vehicles, i => i.a.s.d.VehicleId, p => p.Id, (i, p) => new { i = i, p = p})
                .Select(f => new JsonReportModel() 
                {
                    Date = f.i.a.s.h.SaleDate,
                    DealerId = f.i.a.d.Id,
                    Amount = f.i.a.s.d.Quantity * f.p.Price
                })
                .ToList();

            return collection;
        }

        //static List<ExpenseModel> GetExpenseData()
        //{
        //    List<ExpenseModel> expensesCollection = new List<ExpenseModel>();
        //    using (SQLStoreDb db = new SQLStoreDb())
        //    {
        //        var coll = (from vm in db.VendorMonths
        //                    select new ExpenseModel
        //                    {
        //                        vendor_name = vm.Vendor.VendorName,
        //                        month = vm.Month.MonthDate,
        //                        expenses = vm.Expenses
        //                    }).ToList();


        //        foreach (var exModel in coll)
        //        {
        //            expensesCollection.Add(exModel);
        //        }
        //    }

        //    return expensesCollection;
        //}

        //public static void RecordReports(MongoDBAccess mongodb)
        //{
        //    string path = "Product-Reports\\";
        //    var reportCollection = GetReportData();


        //    mongodb.StoreInReportCollection(reportCollection);

        //    WriteToFileSystem(path, mongodb.GetReportObjects());
        //}

        //public static void RecordExpenses(MongoDBAccess mongodb)
        //{
        //    var expensesCollection = GetExpenseData();
        //    mongodb.StoreInExpensesCollection(expensesCollection);
        //}

    }
}
