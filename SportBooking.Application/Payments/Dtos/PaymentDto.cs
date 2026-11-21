using SportBooking.Domain.Entities;
using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Payments.Dtos
{
    public record PaymentDto(
        int Id,
        int BookingId,
        decimal Amount,
        PaymentMethod Method,
        PaymentStatus Status,
        DateTime? PaidAt,
        string FieldName,
        string VenueName,
        DateTime StartAt,
        DateTime EndAt
        );
    
}
