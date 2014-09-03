namespace PdfReportCreator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Reports;

    public class PdfReportSQLServerGenerator : IReportGenerator
    {
        private IVehicleVendorRepository repository;

        public PdfReportSQLServerGenerator(IVehicleVendorRepository vehicleVendorRepository)
        {
            this.repository = vehicleVendorRepository;
        }

        // Data cant be from the "GeneratePDF" or can be passed out
        private List<IGrouping<string, CarsPdfRow>> GetData()
        {
            var collection = this.repository.Vehicles
                .Select(x => new CarsPdfRow
                {
                    Model = x.Name,
                    Category = x.Category.ToString(),
                    Price = x.Price.ToString()
                })
                .GroupBy(x => x.Category)
                .ToList();

            return collection;
        }

        public void GenerateReport()
        {
            // In case of DateTime usage
            //Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var collection = GetData();
            decimal totalSum = 0;
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(ReportSettings.Default.PDFFileName, FileMode.Create));

            document.Open();

            // It is important to set the columns count, so the appearance is ok
            int tableColumns = 3;

            PdfPTable table = new PdfPTable(tableColumns);
            CellsFactory cells = new CellsFactory(table, tableColumns);

            cells.TitleCell("Nissan Cars Report by Category");

            foreach (var rows in collection)
            {
                cells.HeaderRow("Model", "Category", "Price");

                foreach (var row in rows)
                {
                    cells.DataCellRow(row.Model, row.Category, row.Price.ToString());
                }

                var groupSum = rows.Sum(x => decimal.Parse(x.Price));
                totalSum += groupSum;
                cells.SummaryCell(string.Format("Group Price: {0}", groupSum.ToString()));
            }

            cells.TitleCell(string.Format("Total Price:  {0}", totalSum.ToString()));

            document.Add(table);
            document.Close();
        }
    }
}
