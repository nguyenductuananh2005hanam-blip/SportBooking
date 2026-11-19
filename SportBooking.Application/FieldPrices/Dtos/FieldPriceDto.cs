using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.FieldPrices.Dtos
{
    public record FieldPriceDto
    (
         int Id,
         int FieldId ,
         TimeOnly StartTime ,
         TimeOnly EndTime ,
         DayType DayType ,
         decimal Price 
        );
}
