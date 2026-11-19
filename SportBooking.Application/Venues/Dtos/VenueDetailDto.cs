using SportBooking.Application.Fields.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Venues.Dtos
{
    public record VenueDetailDto
    (
        int Id,
        string Name,
        string Address,
        string? Phone,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        IReadOnlyList<FieldDto> Fields
        );
}
