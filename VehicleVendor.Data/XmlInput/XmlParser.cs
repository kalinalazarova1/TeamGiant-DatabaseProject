namespace VehicleVendor.Data.XmlInput
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Models;

    public class XmlParser
    {
        private const string FileNotFoundExceptionFormat = "The specified {0} file does not exist";
        private XmlSchema schema;

        public XmlParser(string schemaFileLocation)
        {
            this.AddSchema(schemaFileLocation);
        }

        public void ParseDiscountsReader(IVehicleVendorRepository repo, string xmlLocation)
        {
            this.CheckFileExistence(xmlLocation);
            
            // Set validation settings
            var settings = new XmlReaderSettings();
            settings.Schemas.Add(this.schema);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(this.DiscountsValidationEventHandler);

            // Read xml
            using (var discountsReader = XmlReader.Create(xmlLocation, settings))
            {
                var dealer = new Queue<KeyValuePair<string, string>>();
                var ns = discountsReader.NamespaceURI;
                string lastElement = null;
                while (discountsReader.Read())
                {
                    switch (discountsReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (discountsReader.Name == string.Format("{0}discount", ns))
                            {
                                this.GenerateDiscount(repo, dealer);
                            }
                            else
                            {
                                lastElement = discountsReader.LocalName;
                            }

                            break;
                        case XmlNodeType.Text:
                            var value = discountsReader.Value;
                            var pair = new KeyValuePair<string, string>(lastElement, value);
                            dealer.Enqueue(pair);
                            break;
                        case XmlNodeType.EndElement:
                            this.GenerateDiscount(repo, dealer);

                            break;
                    }
                }
            }
        }
        
        [Obsolete]
        public void ParseDiscounts(string xmlLocation)
        {
            this.CheckFileExistence(xmlLocation);

            using (var xmlStream = File.OpenRead(xmlLocation))
            {
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(this.schema);

                var xdoc = XDocument.Load(xmlStream);
                xdoc.Validate(schemas, this.DiscountsValidationEventHandler);
                var ns = xdoc.Root.Name.Namespace;
                var discounts = xdoc.Descendants(ns + "discount")
                                    .Select(dis => new
                                    {
                                        Company = dis.Descendants(ns + "dealer").Elements(ns + "company").First().Value,
                                        Country = dis.Descendants(ns + "dealer").Elements(ns + "country").First().Value,
                                        Amount = dis.Element(ns + "amount").Value
                                    });

                foreach (var discount in discounts)
                {
                    Console.WriteLine(discount.Company);
                }
            }
        }

        private bool AddSchema(string schemaFileLocation)
        {
            this.CheckFileExistence(schemaFileLocation);

            using (var schemaReader = File.OpenRead(schemaFileLocation))
            {
                this.schema = XmlSchema.Read(schemaReader, this.DiscountsValidationEventHandler);
            }

            return true;
        }
 
        private void CheckFileExistence(string pathToFile)
        {
            if (string.IsNullOrEmpty(pathToFile))
            {
                throw new ArgumentException("Specified path is incorrect (null or empty)");
            }

            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException(string.Format(FileNotFoundExceptionFormat, "XSD schema"), pathToFile);
            }
        }

        private Discount GenerateDiscount(IVehicleVendorRepository repo, Queue<KeyValuePair<string, string>> dealerSchema)
        {
            if (dealerSchema.Count < 3)
            {
                return null;
            }

            IEnumerable<Dealer> dealers = null;
            Dealer currentDealer = null;

            while (dealerSchema.Count > 0)
            {
                var dequeuedPair = dealerSchema.Dequeue();

                //Find Dealer and generate Discount entry
                switch (dequeuedPair.Key)
                {
                    case "company":
                        dealers = repo.Dealers.Where(d => d.Company == dequeuedPair.Value);
                        if (dealers == null)
                        {
                            Console.WriteLine(string.Format("No Dealer: {0}", dequeuedPair.Value));
                            return null;
                        }

                        //Console.WriteLine(dequeuedPair.Key + ' ' + dequeuedPair.Value);
                        break;
                    case "country":
                        currentDealer = dealers.Where(d => d.Country.Name == dequeuedPair.Value).First();

                        if (currentDealer == null)
                        {
                            Console.WriteLine(string.Format("No such dealer in this country: {0}", dequeuedPair.Value));
                            return null;
                        }

                        //Console.WriteLine(dequeuedPair.Key + ' ' + dequeuedPair.Value);
                        break;
                    case "amount":
                        // Generate and add new discount
                        repo.Add<Discount>(
                            new Discount()
                            {
                                DealerId = currentDealer.Id,
                                Amount = double.Parse(dequeuedPair.Value)
                            }
                        );

                        Console.WriteLine(currentDealer.Country.Name + " " + currentDealer.Id + " " + currentDealer.Company);
                        Console.WriteLine(dequeuedPair.Key + ' ' + dequeuedPair.Value);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Error in XML parsing! Unknown discount property {0}", dequeuedPair.Key));
                }
            }

            return null;
        }

        private void DiscountsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    throw new XmlSchemaValidationException(string.Format("Validation error: {0}", e.Message));
                case XmlSeverityType.Warning:
                    // Could be logged.
                    break;
            }
        }
    }
}