namespace VehicleVendor.DataAceessData
{
    using System.Linq;
    using Telerik.OpenAccess;

    public interface IVehicleVendorMySqlDbContext : IUnitOfWork
    {
        IQueryable<DataAccessIncome> DataAccessIncomes { get; }

        IQueryable<DataAccessDealer> DataAccessDealers { get; }

        IQueryable<DataAccessCountry> DataAccessCountries { get; }

        void SaveChanges();
    }
}