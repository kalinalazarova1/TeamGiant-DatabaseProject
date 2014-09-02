using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleVendor.Models;

namespace VehicleVendor.Reports.JsonReportSQLServerGenerator
{
    public class JsonReportModel
    {
        //[JsonConverter(typeof(ObjectIdConverter))]
        //public ObjectId _id { get; set; }

        public int ProductId { get; set; }

        public string Model { get; set; }

        public Category Category { get; set; }

        public decimal Price { get; set; }

        //public int total_quantity_sold { get; set; }

        //public decimal total_incomes { get; set; }
    }
}
