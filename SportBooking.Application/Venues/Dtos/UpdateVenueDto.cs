using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Venues.Dtos
{
    public record UpdateVenueDto(
        string Name,
        string Address,
        string? Phone,
        TimeOnly OpenTime,
        TimeOnly CloseTime
     );
    
}
