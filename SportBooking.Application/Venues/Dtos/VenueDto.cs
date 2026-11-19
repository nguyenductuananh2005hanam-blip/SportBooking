using SportBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Venues.Dtos
{
    public record VenueDto(
        int Id,
        string Name,
        string Address,
        string? Phone,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        int FieldCount,
        decimal? MinPrice,
        string? ThumbnailUrl
        );
}


    

