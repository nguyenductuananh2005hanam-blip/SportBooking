using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Venues.Dtos
{
    public record CreateVenueDto
    (
        int Id,
        string Name,
        string Address,
        string? Phone,
        TimeOnly OpenTime,
        TimeOnly CloseTime
    );
}
