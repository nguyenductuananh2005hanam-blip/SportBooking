using SportBooking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportBooking.Web.Models
{
    public class CreateBookingViewModel
    {
        [Required]
        public int FieldId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đặt sân")]
        [Display(Name = "Ngày đặt")]
        public DateOnly BookingDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu")]
        [Display(Name = "Giờ bắt đầu")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc")]
        [Display(Name = "Giờ kết thúc")]
        public TimeOnly EndTime { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        // Danh sách khung giờ đã có người đặt trong ngày để hiển thị tham khảo trên form
        public List<BookedSlotViewModel> BookedSlots { get; set; } = new();
    }

    public class BookedSlotViewModel
    {
        public int BookingId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public BookingStatus Status { get; set; }
    }

    public class BookingItemViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string VenueAddress { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
