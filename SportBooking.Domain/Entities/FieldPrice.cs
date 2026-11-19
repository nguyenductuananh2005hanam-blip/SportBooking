using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Domain.Entities
{
    public class FieldPrice
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayType DayType { get; set; } = DayType.Weekday;
        public decimal Price { get; set; } // VND / giờ

        public Field Field { get; set; } = null!;
    }
}
