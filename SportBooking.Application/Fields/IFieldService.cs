using SportBooking.Application.FieldPrices.Dtos;
using SportBooking.Application.Fields.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Fields
{
    public interface IFieldService
    {
        Task<FieldDetailDto?> GetAllAsync(int id, CancellationToken ct = default);
    }
}
