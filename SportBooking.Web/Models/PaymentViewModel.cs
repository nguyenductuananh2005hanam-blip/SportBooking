using Microsoft.AspNetCore.Mvc.ModelBinding;
using SportBooking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportBooking.Web.Models
{
    public class PaymentViewModel
    {
        [Required]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod? Method { get; set; }

        [BindNever] public string FieldName { get; set; } = string.Empty;
        [BindNever] public string VenueName { get; set; } = string.Empty;
        [BindNever] public DateTime StartAt { get; set; }
        [BindNever] public DateTime EndAt { get; set; }
        [BindNever] public decimal Amount { get; set; }
    }
}
