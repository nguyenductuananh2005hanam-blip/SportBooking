using Microsoft.EntityFrameworkCore;
using SportBooking.Application.Bookings.Dtos;
using SportBooking.Application.Common.Exceptions;
using SportBooking.Application.Common.Interfaces;
using SportBooking.Domain.Entities;
using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly IApplicationDbContext _ctx; 

        public BookingService(IApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BookingDto> CreateAsync(int userId, CreateBookingDto dto, CancellationToken ct = default)
        {
            // 1. Validate thời gian cơ bản
            if (dto.StartAt >= dto.EndAt)
                throw new BusinessException("Thời gian kết thúc phải lớn hơn thời gian bắt đầu");

            if (dto.StartAt.Date != dto.EndAt.Date)
                throw new BusinessException("Chỉ hỗ trợ đặt sân trong cùng một ngày");

            if (dto.StartAt <= DateTime.Now)
                throw new BusinessException("Không thể đặt khung giờ trong quá khứ");

            var duration = dto.EndAt - dto.StartAt;
            if (duration.TotalMinutes < 30)
                throw new BusinessException("Thời lượng đặt sân tối thiểu là 30 phút");

            // 2. Lấy thông tin sân, cụm sân và bảng giá
            var field = await _ctx.Fields
                .AsNoTracking()
                .Include(f => f.Venue)
                .Include(f => f.Prices)
                .FirstOrDefaultAsync(f => f.Id == dto.FieldId, ct);

            if (field is null || !field.IsActive || !field.Venue.IsActive)
                throw new BusinessException("Sân không tồn tại hoặc hiện đang tạm ngưng hoạt động");

            // 3. Kiểm tra khung giờ có nằm trong giờ hoạt động của cụm sân không
            var startTime = TimeOnly.FromDateTime(dto.StartAt);
            var endTime = TimeOnly.FromDateTime(dto.EndAt);

            if (startTime < field.Venue.OpenTime || endTime > field.Venue.CloseTime)
                throw new BusinessException(
                    $"Khung giờ đặt phải nằm trong giờ mở cửa của sân ({field.Venue.OpenTime:HH\\:mm} - {field.Venue.CloseTime:HH\\:mm})");

            // 4. Kiểm tra trùng lịch (chỉ tính các đơn chưa bị hủy)
            var isOverlapped = await _ctx.Bookings
                .AsNoTracking()
                .AnyAsync(b =>
                    b.FieldId == dto.FieldId &&
                    b.Status != BookingStatus.Cancelled &&
                    b.StartAt < dto.EndAt &&
                    b.EndAt > dto.StartAt, ct);

            if (isOverlapped)
                throw new ConflictException("Khung giờ này đã có người đặt, vui lòng chọn khung giờ khác");

            // 5. Tính tổng tiền tự động dựa trên bảng giá theo khung giờ (FieldPrices)
            var totalPrice = CalculateTotalPrice(field.Prices, startTime, endTime);

            var note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();

            var booking = new Booking
            {
                UserId = userId,
                FieldId = dto.FieldId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };

            _ctx.Bookings.Add(booking);

            try
            {
                await _ctx.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("Không thể tạo đơn đặt sân do xung đột dữ liệu, vui lòng thử lại");
            }

            // Truy vấn lại DTO đầy đủ thông tin để trả về
            var created = await GetByIdAsync(booking.Id, userId, isAdmin: true, ct);
            return created!;
        }

        public async Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync(int userId, CancellationToken ct = default)
        {
            return await _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartAt)
                .Select(b => new BookingDto(
                    b.Id,
                    b.UserId,
                    b.User.FullName,
                    b.User.Phone,
                    b.FieldId,
                    b.Field.Name,
                    b.Field.Venue.Name,
                    b.Field.Venue.Address,
                    b.StartAt,
                    b.EndAt,
                    b.TotalPrice,
                    b.Status,
                    b.Note,
                    b.CreatedAt))
                .ToListAsync(ct);
        }

        public async Task<BookingDto?> GetByIdAsync(int id, int userId, bool isAdmin = false, CancellationToken ct = default)
        {
            var query = _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.Id == id);

            if (!isAdmin)
            {
                query = query.Where(b => b.UserId == userId);
            }

            return await query
                .Select(b => new BookingDto(
                    b.Id,
                    b.UserId,
                    b.User.FullName,
                    b.User.Phone,
                    b.FieldId,
                    b.Field.Name,
                    b.Field.Venue.Name,
                    b.Field.Venue.Address,
                    b.StartAt,
                    b.EndAt,
                    b.TotalPrice,
                    b.Status,
                    b.Note,
                    b.CreatedAt))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IReadOnlyList<BookedSlotDto>> GetBookedSlotsAsync(int fieldId, DateOnly date, CancellationToken ct = default)
        {
            var dayStart = date.ToDateTime(TimeOnly.MinValue);
            var dayEnd = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

            return await _ctx.Bookings
                .AsNoTracking()
                .Where(b =>
                    b.FieldId == fieldId &&
                    b.Status != BookingStatus.Cancelled &&
                    b.StartAt < dayEnd &&
                    b.EndAt > dayStart)
                .OrderBy(b => b.StartAt)
                .Select(b => new BookedSlotDto(
                    b.Id,
                    b.StartAt,
                    b.EndAt,
                    b.Status))
                .ToListAsync(ct);
        }

        public async Task CancelAsync(int id, int userId, bool isAdmin = false, CancellationToken ct = default)
        {
            var booking = await _ctx.Bookings.FirstOrDefaultAsync(b => b.Id == id, ct);

            if (booking is null)
                throw new BusinessException("Không tìm thấy đơn đặt sân");

            if (!isAdmin && booking.UserId != userId)
                throw new BusinessException("Bạn không có quyền hủy đơn đặt sân này");

            if (booking.Status == BookingStatus.Cancelled)
                throw new BusinessException("Đơn đặt sân này đã được hủy trước đó");

            if (booking.Status == BookingStatus.Completed)
                throw new BusinessException("Không thể hủy đơn đặt sân đã hoàn thành");

            if (!isAdmin && booking.StartAt <= DateTime.Now)
                throw new BusinessException("Không thể hủy đơn khi đã đến hoặc quá giờ bắt đầu đá");

            booking.Status = BookingStatus.Cancelled;
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateStatusAsync(int id, BookingStatus newStatus, CancellationToken ct = default)
        {
            var booking = await _ctx.Bookings.FirstOrDefaultAsync(b => b.Id == id, ct);

            if (booking is null)
                throw new BusinessException("Không tìm thấy đơn đặt sân");

            if (booking.Status == BookingStatus.Cancelled)
                throw new BusinessException("Không thể thay đổi trạng thái của đơn đã hủy");

            booking.Status = newStatus;
            await _ctx.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Tính tiền linh hoạt kể cả khi khung giờ đặt vắt ngang qua nhiều mức giá khác nhau
        /// (Ví dụ: đặt 16:00 - 18:00, trong đó 16:00-17:00 giá 200k/h, 17:00-18:00 giá 300k/h => tổng 500k)
        /// </summary>
        private static decimal CalculateTotalPrice(
            IEnumerable<FieldPrice> priceRules,
            TimeOnly startTime,
            TimeOnly endTime)
        {
            decimal totalPrice = 0m;
            double coveredMinutes = 0;
            var requestedMinutes = (endTime - startTime).TotalMinutes;

            foreach (var rule in priceRules)
            {
                var overlapStart = startTime > rule.StartTime ? startTime : rule.StartTime;
                var overlapEnd = endTime < rule.EndTime ? endTime : rule.EndTime;

                if (overlapEnd > overlapStart)
                {
                    var minutes = (overlapEnd - overlapStart).TotalMinutes;
                    coveredMinutes += minutes;
                    totalPrice += ((decimal)minutes / 60m) * rule.Price;
                }
            }

            // Đảm bảo toàn bộ thời gian khách đặt đều đã được cấu hình đơn giá
            //if (Math.Abs(coveredMinutes - requestedMinutes) > 0.1)
            //    throw new BusinessException("Một phần khung giờ bạn chọn chưa được cấu hình đơn giá thuê sân");

            return Math.Round(totalPrice, 0);
        }
    }
}
