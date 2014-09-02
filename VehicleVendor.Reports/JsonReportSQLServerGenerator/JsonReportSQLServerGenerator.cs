using System.Collections.Generic;
using VehicleVendor.Data;
using VehicleVendor.Data.Repositories;
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
                    ProductId = rep.ProductId,
                    Category = rep.Category,
                    Model = rep.Model,
                    Price = rep.Price
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                using (StreamWriter sw = new StreamWriter(path + rep.ProductId + ".json"))
                {
                    sw.WriteLine(json);
                }
            }
        }

        private List<JsonReportModel> GetReportData()
        {
            var collection = this.repository.Vehicles
                .Select(x => new JsonReportModel
                {
                    ProductId = x.Id,
                    Model = x.Name,
                    Category = x.Category,
                    Price = x.Price
                }).ToList();

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
