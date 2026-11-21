using Microsoft.EntityFrameworkCore;
using SportBooking.Application.Common.Exceptions;
using SportBooking.Application.Common.Interfaces;
using SportBooking.Application.Payments.Dtos;
using SportBooking.Domain.Entities;
using SportBooking.Domain.Enums;

namespace SportBooking.Application.Payments;

public class PaymentService : IPaymentService
{
    public readonly IApplicationDbContext _db;

    public PaymentService(IApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var booking = await _db.Bookings
        .Include(b => b.Field).ThenInclude(f => f.Venue)
        .FirstOrDefaultAsync(b => b.Id == dto.BookingId, ct)
        ?? throw new NotFoundException("Không tìm thấy sân đặt");

        if (booking.Status != BookingStatus.Pending)
            throw new ConflictException("Đơn đặt sân không còn ở trạng thái chờ thanh toán.");

        var payments = await _db.Payments
            .Where(p => p.BookingId == booking.Id)
            .ToListAsync(ct);

        if (payments.Any(p => p.Status == PaymentStatus.Paid))
            throw new ConflictException("Đơn đặt sân đã được thanh toán.");

        var payment = payments.FirstOrDefault(p => p.Status == PaymentStatus.Pending);
        if (payment is null)
        {
            payment = new Payment
            {
                BookingId = booking.Id,
                Status = PaymentStatus.Pending,
                PaidAt = null
            };
            _db.Payments.Add(payment);
        }

        payment.Method = dto.Method;
        payment.Amount = booking.TotalPrice;   

        await _db.SaveChangesAsync(ct);
        return ToDto(payment);

    }

    public async Task<PaymentDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var payment = await _db.Payments
       .AsNoTracking()
       .Include(p => p.Booking).ThenInclude(b => b.Field).ThenInclude(f => f.Venue)
       .FirstOrDefaultAsync(p => p.Id == id, ct)
       ?? throw new NotFoundException("Không tìm thấy thanh toán");

        return ToDto(payment);
    }

    private static PaymentDto ToDto(Payment p) => new PaymentDto(p.Id, p.BookingId, p.Amount, p.Method, p.Status, p.PaidAt, p.Booking.Field.Name, p.Booking.Field.Venue.Name, p.Booking.StartAt, p.Booking.EndAt);
       
}