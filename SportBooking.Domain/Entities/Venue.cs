using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Domain.Entities
{
    public class Venue
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public TimeOnly OpenTime { get; set; } = new TimeOnly(5, 0);
        public TimeOnly CloseTime { get; set; } = new TimeOnly(23, 0);
        public bool IsActive { get; set; } = true;

        public User Owner { get; set; } = null!;
        public ICollection<Field> Fields { get; set; } = new List<Field>();
    }
}
