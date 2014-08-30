namespace VehicleVendor.Data
{
    using System;
    using System.Collections.Generic;
    using VehicleVendor.Data;
    using VehicleVendor.Data.Repositories;
    using VehicleVendor.Models;

    public class VehicleVendorData : IVehicleVendorData
    {
        private IVehicleVendorDbContext context;
        private IDictionary<Type, object> repositories;

        public VehicleVendorData()
            : this(new VehicleVendorDbContext())
        {
        }

        public VehicleVendorData(IVehicleVendorDbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IGenericRepository<Vehicle> Vehicles
        {
            get
            {
                return this.GetRepository<Vehicle>();
            }
        }

        public IGenericRepository<Dealer> Dealers
        {
            get
            {
                return this.GetRepository<Dealer>();
            }
        }

        public IGenericRepository<Country> Countries
        {
            get
            {
                return this.GetRepository<Country>();
            }
        }

        public IGenericRepository<Sale> Sales
        {
            get
            {
                return this.GetRepository<Sale>();
            }
        }

        public IGenericRepository<SaleDetails> SaleDetails
        {
            get
            {
                return this.GetRepository<SaleDetails>();
            }
        }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        private IGenericRepository<T> GetRepository<T>() where T : class
        {
            var typeOfModel = typeof(T);
            if (!this.repositories.ContainsKey(typeOfModel))
            {
                var type = typeof(GenericRepository<T>);
                this.repositories.Add(typeOfModel, Activator.CreateInstance(type, this.context));
            }

            return (IGenericRepository<T>)this.repositories[typeOfModel];
        }
    }
}
