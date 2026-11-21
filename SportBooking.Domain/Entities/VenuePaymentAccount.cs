using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Domain.Entities
{
    public class VenuePaymentAccount
    {
        public int Id { get; set; }

        public int VenueId { get; set; }
        public Venue Venue { get; set; } = null!;

        public string BankBin { get; set; } = null!;        
        public string BankName { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string AccountName { get; set; } = null!;

        public string? QrImageUrl { get; set; }             

        public bool IsDefault { get; set; }               
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
