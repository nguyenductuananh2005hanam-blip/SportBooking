using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Domain.Entities
{
    public class Field
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public FieldType FieldType { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public Venue Venue { get; set; } = null!;
        public ICollection<FieldPrice> Prices { get; set; } = new List<FieldPrice>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
