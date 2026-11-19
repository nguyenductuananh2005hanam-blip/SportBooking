using SportBooking.Domain.Enums;

namespace SportBooking.Web.Models
{
    public class FieldPricesViewModel
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayType DayType { get; set; }
        public decimal Price { get; set; }
    }
}
