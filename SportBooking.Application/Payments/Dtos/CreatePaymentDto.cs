using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Payments.Dtos
{
    public record CreatePaymentDto
    (
        int BookingId,
        PaymentMethod Method
        );
}
