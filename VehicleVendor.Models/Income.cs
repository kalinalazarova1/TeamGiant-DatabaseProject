﻿namespace VehicleVendor.Models
{
    using System;

    public class Income
    {
        public Income()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public int DealerId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public virtual Dealer Dealer { get; set; }
    }
}
