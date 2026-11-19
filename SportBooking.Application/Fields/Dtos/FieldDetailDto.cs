using SportBooking.Application.FieldPrices.Dtos;
using SportBooking.Domain.Entities;
using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Fields.Dtos
{
    public record FieldDetailDto(
        int id,
        string VenueName,
        string Name,
        FieldType FieldType,
        bool isActive,
        string ImageUrl,
        IReadOnlyList<FieldPriceDto> FieldPrices
        );
}
