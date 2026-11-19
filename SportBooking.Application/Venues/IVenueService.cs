using SportBooking.Application.Venues.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Venues
{
    public interface IVenueService
    {
        Task<IReadOnlyList<VenueDto>> GetAllAsync(CancellationToken ct = default);
        Task<VenueDetailDto?> GetDetailAsync(int id, CancellationToken ct = default);

        Task<VenueDto> CreateVenueAsync(int ownerId, CreateVenueDto dto, CancellationToken ct = default);
        Task<VenueDto> EditVenueAsync(int userId, int id, UpdateVenueDto dto, CancellationToken ct = default);
    }
}
