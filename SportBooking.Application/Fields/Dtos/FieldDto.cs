using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Fields.Dtos
{
    public record FieldDto(
        int id,
        string VenueName,
        string Name,
        FieldType FieldType,
        bool isActive,
        string ImageUrl
        );
    
}
