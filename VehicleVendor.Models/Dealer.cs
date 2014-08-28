﻿namespace VehicleVendor.Models
{
    public class Dealer
    {
        public int Id { get; set; }

        public string Company { get; set; }

        public string Address { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}