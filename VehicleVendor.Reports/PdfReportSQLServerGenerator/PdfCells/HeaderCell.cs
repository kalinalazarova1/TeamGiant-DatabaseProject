namespace PdfReportCreator.PdfCells
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    public class HeaderCell : PdfCell
    {
        public HeaderCell(PdfPTable table, string data)
            : base(table, data, Color.LIGHT_GRAY, 0)
        {
        }
    }
}
