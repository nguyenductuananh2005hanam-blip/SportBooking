using SportBooking.Domain.Enums;

namespace SportBooking.Web.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Ngaytao { get; set; } = string.Empty;
    }
}
