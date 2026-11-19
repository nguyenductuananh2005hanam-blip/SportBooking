namespace SportBooking.Web.Models
{
    public class VenuesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public TimeOnly OpenTime { get; set; }   // Đổi thành TimeSpan nếu Entity/DTO dùng TimeSpan
        public TimeOnly CloseTime { get; set; }  // Đổi thành TimeSpan nếu Entity/DTO dùng TimeSpan
        public int FieldCount { get; set; }
        public decimal? MinPrice { get; set; }
        public string? ImageUrl { get; set; }

        // Helper hỗ trợ hiển thị gọn trên View
        public string OperatingHours => $"{OpenTime:HH\\:mm} - {CloseTime:HH\\:mm}";
        public string PriceDisplay => MinPrice.HasValue
            ? $"Từ {MinPrice.Value:N0} đ"
            : "Liên hệ";
    }
}
