namespace VehicleVendor.Models
{
    public class SaleDetails
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
