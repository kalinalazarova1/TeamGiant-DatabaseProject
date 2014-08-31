namespace VehicleVendor.Reports
{
    using System;
    using System.Data.OleDb;
    using System.Data.SQLite;
    using System.Linq;
    using VehicleVendor.Data.Repositories;

    public class ExcelReportsSQLiteGenerator : IReportGenerator
    {
        private IVehicleVendorRepository repo;
        private DateTime start;
        private DateTime end;

        public ExcelReportsSQLiteGenerator(IVehicleVendorRepository repo, DateTime start, DateTime end)
        {
            this.repo = repo;
            this.start = start;
            this.end = end;
        }

        public void GenerateReport()
        {

            var dbConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\\..\\Report.xlsx;Extended Properties=\"Excel 12.0 Xml;HDR=YES\";");
            dbConn.Open();
            var con = new SQLiteConnection(@"Data Source=..\..\..\SQLiteDatabase\nissan_costs.sqlite;Version=3;");
            con.Open();
            using (con)
            {
                using (dbConn)
                {
                    var cmdEx = new OleDbCommand();
                    cmdEx.Connection = dbConn;
                    cmdEx.CommandText = "CREATE TABLE ProfitReport (Dealer char(255), Sales currency, Costs currency, Profit currency)";
                    cmdEx.ExecuteNonQuery();
                    
                    var cmd = new SQLiteCommand(
                        "SELECT * FROM dealersCosts",
                        con);
                    var reader = cmd.ExecuteReader();
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            string dealer = (string)reader["Dealer"];
                            decimal sales = 0m;
                            var records1 = this.repo.Sales
                                .Where(s => s.SaleDate <= this.end && s.SaleDate >= this.start && s.Dealer.Company == dealer);
                            var records = this.repo.Sales
                                .Where(s => s.SaleDate <= this.end && s.SaleDate >= this.start && s.Dealer.Company == dealer)
                                .Join(this.repo.SalesDetails, h => h.Id, d => d.SaleId, (h, d) => new { h = h, d = d })
                                .Join(this.repo.Vehicles, s => s.d.Vehicle.Id, v => v.Id, (s, v) => new { s = s, v = v });

                            if(records.Count() > 0)
                            {
                                sales = records.Sum(r => r.s.d.Quantity * r.v.Price);
                            }

                            decimal costs = ((decimal)reader["ConstCosts"]) / 30m * (decimal)(this.end - this.start).TotalDays + (decimal)reader["SaleCosts"] * sales;
                            decimal profit = (decimal)(sales - costs);
                            var excelCmd = new OleDbCommand("INSERT INTO [ProfitReport$] (Dealer, Sales, Costs, Profit) VALUES(@dealer, @sales, @costs, @profit)", dbConn);
                            excelCmd.Parameters.AddWithValue("@dealer", dealer);
                            excelCmd.Parameters.AddWithValue("@sales", sales);
                            excelCmd.Parameters.AddWithValue("@costs", costs);
                            excelCmd.Parameters.AddWithValue("@profit", profit);
                            excelCmd.Connection = dbConn;
                            excelCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
