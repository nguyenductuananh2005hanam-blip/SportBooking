using Microsoft.EntityFrameworkCore;
using SportBooking.Application.Common.Exceptions;
using SportBooking.Application.Common.Interfaces;
using SportBooking.Application.Fields.Dtos;
using SportBooking.Application.Venues.Dtos;
using SportBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Venues
{
    public class VenueService : IVenueService
    {
        public readonly IApplicationDbContext _ctx;
        public readonly IImageStorageService _img;

        public VenueService(IApplicationDbContext ctx, IImageStorageService img)
        {
            _ctx = ctx;
            _img = img;
        }

        public Task<VenueDto> CreateVenueAsync(int ownerId, CreateVenueDto dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<VenueDto> EditVenueAsync(int userId, int id, UpdateVenueDto dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<VenueDto>> GetAllAsync(CancellationToken ct = default)
        {
            var query = _ctx.Venues
           .AsNoTracking()
           .Where(v => v.IsActive);

            return await query
                .OrderBy(v => v.Name)
                .ThenBy(v => v.Id)
                .Select(v => new VenueDto(
                    v.Id,
                    v.Name,
                    v.Address,
                    v.Phone,
                    v.OpenTime,
                    v.CloseTime,
                    v.Fields.Count(f => f.IsActive),
                    v.Fields
                        .Where(f => f.IsActive)
                        .SelectMany(f => f.Prices)
                        .Min(p => (decimal?)p.Price),
                    v.Fields
                        .Where(f => f.IsActive && f.ImageUrl != null)
                        .OrderBy(f => f.Id)
                        .Select(f => f.ImageUrl)
                        .FirstOrDefault()))
                .ToListAsync(ct);
        }


        public async Task<VenueDetailDto?> GetDetailAsync(int id, CancellationToken ct = default)
        {
            var venue = await _ctx.Venues
                                    .AsNoTracking()
                                    .Include(v => v.Fields.Where(f => f.IsActive).OrderBy(f => f.Name))
                                    .FirstOrDefaultAsync(v => v.Id == id && v.IsActive, ct);

            return venue is null ? null : ToDto(venue);
        }
        private static VenueDetailDto ToDto(Venue v) => new(
            v.Id, v.Name, v.Address, v.Phone, v.OpenTime, v.CloseTime,
            v.Fields
                .Select(f => new FieldDto(f.Id, v.Name, f.Name, f.FieldType,f.IsActive ,f.ImageUrl))
                .ToList());

    }
}
