using SportBooking.Application.Bookings.Dtos;
using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Bookings
{
    public interface IBookingService
    {
        Task<BookingDto> CreateAsync(int userId, CreateBookingDto dto, CancellationToken ct = default);
        Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync(int userId, CancellationToken ct = default);
        Task<BookingDto?> GetByIdAsync(int id, int userId, bool isAdmin = false, CancellationToken ct = default);
        Task<IReadOnlyList<BookedSlotDto>> GetBookedSlotsAsync(int fieldId, DateOnly date, CancellationToken ct = default);
        Task CancelAsync(int id, int userId, bool isAdmin = false, CancellationToken ct = default);
        Task UpdateStatusAsync(int id, BookingStatus newStatus, CancellationToken ct = default);
    }
}
