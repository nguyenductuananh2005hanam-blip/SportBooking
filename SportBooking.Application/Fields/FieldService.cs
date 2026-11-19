using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SportBooking.Application.Common.Interfaces;
using SportBooking.Application.FieldPrices.Dtos;
using SportBooking.Application.Fields.Dtos;
using SportBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Fields
{
    public class FieldService : IFieldService
    {
        public readonly IApplicationDbContext _ctx;

        public FieldService(IApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<FieldDetailDto?> GetAllAsync(int id, CancellationToken ct = default)
        {
            var field = await _ctx.Fields
             .AsNoTracking()
             .Include(f => f.Venue)
             .Include(f => f.Prices.OrderBy(p => p.DayType).ThenBy(p => p.StartTime))
             .FirstOrDefaultAsync(f => f.Id == id && f.IsActive && f.Venue.IsActive, ct);

            return field is null ? null : ToDto(field);
        }
        private static FieldDetailDto ToDto(Field f) => new(
        f.Id, f.Venue.Name, f.Name, f.FieldType,f.IsActive, f.ImageUrl,
        f.Prices
            .Select(p => new FieldPriceDto(p.Id,p.FieldId, p.StartTime, p.EndTime, p.DayType, p.Price))
            .ToList());

    }
}
