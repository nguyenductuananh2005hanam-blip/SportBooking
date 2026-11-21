using System;
using System.Collections.Generic;
using System.Text;
using SportBooking.Application.Payments.Dtos;

namespace SportBooking.Application.Payments
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken ct = default);

        Task<PaymentDto> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
