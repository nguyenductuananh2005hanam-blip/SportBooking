using SportBooking.Domain.Enums;

namespace SportBooking.Web.Models
{
    public class BookingHistoriedViewModel
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public string? Note { get; set; }
        public string CreatedAt { get; set; } 
    }
}
