namespace VehicleVendor.DataAceessData.Repository
{
    using System.Linq;
    using Telerik.OpenAccess;
    using VehicleVendor.DataAceessData;

    public class VehicleVendorMySqlRepository : IVehicleVendorMySqlRepository
    {
        private IVehicleVendorMySqlDbContext context;

        public VehicleVendorMySqlRepository(IVehicleVendorMySqlDbContext context)
        {
            this.context = context;
        }

        public IQueryable<DataAccessDealer> DataAccessDealers
        {
            get
            {
                return this.context.DataAccessDealers;
            }
        }

        public IQueryable<DataAccessIncome> DataAccessIncomes
        {
            get
            {
                return this.context.DataAccessIncomes;
            }
        }

        public IQueryable<DataAccessCountry> DataAccessCountries
        {
            get
            {
                return this.context.DataAccessCountries;
            }
        }

        public void Add<T>(T entity) where T : class
        {
            this.context.Add(entity);
        }

        public void Update<T>(T old, T entity) where T : class
        {
            this.context.Delete(old);
            this.context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this.context.Add(entity);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
