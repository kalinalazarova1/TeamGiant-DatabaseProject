namespace PdfReportCreator
{
    using iTextSharp.text.pdf;
    using PdfReportCreator.PdfCells;

    public class CellsFactory
    {
        private PdfPTable table;
        private int columnsCount;

        public CellsFactory(PdfPTable table, int columnsCount)
        {
            this.table = table;
            this.columnsCount = columnsCount;
        }

        public void DataCell(string data)
        {
            var dataCell = new DataCell(this.table, data);
        }

        public void SummaryCell(string data)
        {
            var summaryCell = new SummaryCell(this.table, data);
        }

        public void HeaderCell(string data)
        {
            var summaryCell = new HeaderCell(this.table, data);
        }

        public void TitleCell(string data)
        {
            var summaryCell = new TitleCell(this.table, data, this.columnsCount);
        }

        public void HeaderRow(params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                this.HeaderCell(name);
            }
        }

        public void DataCellRow(params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                this.DataCell(name);
            }
        }
    }
}
