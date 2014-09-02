namespace VehicleVendor.Reports
{
    using System;
    using System.Data.SQLite;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using VehicleVendor.Data.Repositories;

    public class ExcelReportsSQLiteGenerator : IReportGenerator
    {
        private IVehicleVendorMySqlRepository repoMySql;
        private DateTime start;
        private DateTime end;

        public ExcelReportsSQLiteGenerator(IVehicleVendorMySqlRepository repoMySql, DateTime start, DateTime end)
        {
            this.repoMySql = repoMySql;
            this.start = start;
            this.end = end;
        }

        public void GenerateReport()
        {
            var fileName = "Report" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".xlsx";
            var outputDir = @"../../Reports/";

            var filePath = new FileInfo(outputDir + fileName);
            using (var package = new ExcelPackage(filePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Results(" + this.start.ToShortDateString() + "," + this.end.ToShortDateString() + ")");
                worksheet.Row(1).Height = 20;
                worksheet.Row(2).Height = 18;

                worksheet.Cells[1, 1].Value = "Report For Results By Dealers from " + this.start.ToShortDateString() + " to " + this.end.ToShortDateString();
                var headerRange = worksheet.Cells[1, 1, 1, 4];
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.Size = 16;
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells[2, 1].Value = "Dealer Company";
                worksheet.Cells[2, 2].Value = "Sales in EUR";
                worksheet.Cells[2, 3].Value = "Costs in EUR";
                worksheet.Cells[2, 4].Value = "Results in EUR";

                using (var range = worksheet.Cells[2, 1, 2, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    range.Style.Font.Color.SetColor(Color.WhiteSmoke);
                    range.Style.ShrinkToFit = false;
                }

                var conSQLite = new SQLiteConnection(ReportSettings.Default.SQLiteConnectionString);
                conSQLite.Open();
                using(conSQLite)
                {
                    var sqliteCmd = new SQLiteCommand("SELECT * FROM dealersCosts", conSQLite);
                    var costsReader = sqliteCmd.ExecuteReader();
                    int rowNumber = 3;

                    using(costsReader)
                    {
                        while(costsReader.Read())
                        {
                            string dealer = (string)costsReader["Dealer"];
                            decimal sales = 0m;
                            var records = this.repoMySql.Incomes.Where(i => i.Dealer.Company == dealer);
                                
                            if (records.Count() > 0)
                            {
                                sales = records.Sum(r => r.Amount);
                            }

                            decimal costs = ((decimal)costsReader["ConstCosts"]) / 30m * (decimal)(this.end - this.start).TotalDays + (decimal)costsReader["SaleCosts"] * sales;
                            decimal profit = (decimal)(sales - costs);
                            
                            worksheet.Cells[rowNumber, 1].Value = dealer;
                            worksheet.Cells[rowNumber, 2].Value = sales;
                            worksheet.Cells[rowNumber, 3].Value = costs;
                            worksheet.Cells[rowNumber, 4].Value = profit;
                            worksheet.Cells[rowNumber, 2, rowNumber, 4].Style.Numberformat.Format = "### ### ### ###";

                            using (var rowRange = worksheet.Cells[rowNumber, 1, rowNumber, 4])
                            {
                                rowRange.Style.Font.Bold = false;
                                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rowRange.Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                                rowRange.Style.Font.Color.SetColor(Color.Black);
                                rowRange.Style.ShrinkToFit = false;
                                rowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                rowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                rowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            }

                            rowNumber++;
                        }
                    }

                    worksheet.Column(1).Width = 50;
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    package.Save();
                }
            }
        }
    }
}
