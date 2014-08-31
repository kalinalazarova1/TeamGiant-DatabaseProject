namespace VehicleVendor.Data
{
    using System.Collections.Generic;
    using MongoDB.Bson;

    public interface IVehicleVendorMongoDb
    {
        IEnumerable<BsonDocument> GetDocument(string documentName);
    }
}
