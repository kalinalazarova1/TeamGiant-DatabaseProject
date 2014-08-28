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

    public class VehicleVendorConsoleClinetEntryPoint
    {
        public static void Main(string[] args)
        {
            var db = new VendorContext();
            db.Database.Initialize(true);
        }
    }
}
