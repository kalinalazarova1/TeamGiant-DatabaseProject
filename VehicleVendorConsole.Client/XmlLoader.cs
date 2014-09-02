namespace VehicleVendorConsole.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Models;

    public class XmlLoader
    {
        private readonly IVehicleVendorRepository repo;

        public XmlLoader(IVehicleVendorRepository repo)
        {
            this.repo = repo;
        }

        public void LoadDiscounts(IDictionary<int, double> discountParameters)
        {
            foreach (var discount in discountParameters)
            {
                this.repo.Add<Discount>(
                    new Discount(){ Amount = discount.Value, DealerId = discount.Key });
            }
        }
    }
}