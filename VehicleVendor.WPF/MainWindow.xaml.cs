using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VehicleVendor.Data;
using VehicleVendor.Data.Repositories;
using VehicleVendor.Models;
using VehicleVendor.Reports;
using PdfReportCreator;
using VehicleVendorConsole.Client;
using VehicleVendorConsole.Client.XmlInput;

namespace VehicleVendor.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IVehicleVendorRepository repo;
        private IVehicleVendorMySqlRepository repoMySql;
        private IVehicleVendorMongoDb nissanMongoDb;

        public MainWindow()
        {
            InitializeComponent();
            this.repo = new VehicleVendorRepository(new IVehicleVendorDbContext[] { new VehicleVendorDbContext() });
            this.repoMySql = new VehicleVendorMySqlRepository(new VehicleVendorMySqlDbContext());
            this.nissanMongoDb = new VehicleVendorMongoDb();
        }

        public void OnLoadMongoClick(object sender, RoutedEventArgs e)
        {
            var mongoLoader = new MongoLoader(this.repo, this.nissanMongoDb);
            mongoLoader.LoadRepository();
            repo.SaveChanges();
        }

        public void OnXMLToSQLClick(object sender, RoutedEventArgs e)
        {
            var mongoLoader = new MongoLoader(this.repo, this.nissanMongoDb);
            mongoLoader.LoadRepository();
            var xmlimporter = new XmlImporter(repo);
            var parseResult = xmlimporter.ParseDiscounts(@"..\..\..\Discounts.xml", @"..\..\..\Discounts.xsd");
            mongoLoader.LoadDiscounts(parseResult);
            repo.SaveChanges();
        }

        public void OnXMLToMongoClick(object sender, RoutedEventArgs e)
        {
            var xmlimporter = new XmlImporter(repo);
            var parseResult = xmlimporter.ParseDiscounts(@"..\..\..\Discounts.xml", @"..\..\..\Discounts.xsd");
            var mongoLoader = new MongoLoader(this.repo, this.nissanMongoDb);
            mongoLoader.LoadDiscountsInMongo(parseResult);
        }

        public void OnLoadExcelZipClick(object sender, RoutedEventArgs e)
        {
            var zipExLoader = new ZipExcelLoader(repo);
            zipExLoader.LoadRepository();
            repo.SaveChanges();
        }

        public void OnGenerateExcelClick(object sender, RoutedEventArgs e)
        {
            var excelReporter = new ExcelReportsSQLiteGenerator(repo, new DateTime(2014, 8, 1), new DateTime(2014, 9, 1));
            excelReporter.GenerateReport();
        }

        public void OnGeneratePdfReportClick(object sender, RoutedEventArgs e)
        {
            GeneratePDF pdf = new GeneratePDF(repo);
            pdf.Report("../../PdfReport.pdf");
        }

        public void OnGenerateJSONClick(object sender, RoutedEventArgs e)
        {

        }

        public void OnGenerateXMLReportClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
