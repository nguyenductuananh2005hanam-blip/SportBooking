using SportBooking.Domain.Enums;

namespace SportBooking.Web.Models
{
    public class FieldsViewModel
    {
        public int id { get; set; }
        public string VenueName { get; set; }
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; }
        public bool isActive { get; set; }
        public string ImageUrl { get; set; }

    }
}
