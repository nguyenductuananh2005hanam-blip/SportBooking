using SportBooking.Domain.Entities;
using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Bookings.Dtos
{
    public record CreateBookingDto(
    int FieldId,
    DateTime StartAt,
    DateTime EndAt,
    string? Note);

    // Dữ liệu trả về cho danh sách lịch sử đặt sân & chi tiết đơn
    public record BookingDto(
        int Id,
        int UserId,
        string UserName,
        string UserPhone,
        int FieldId,
        string FieldName,
        string VenueName,
        string VenueAddress,
        DateTime StartAt,
        DateTime EndAt,
        decimal TotalPrice,
        BookingStatus Status,
        string? Note,
        DateTime CreatedAt);

    // Trả về các khung giờ đã có người đặt trong ngày để giao diện khóa/hiển thị lịch trống
    public record BookedSlotDto(
        int BookingId,
        DateTime StartAt,
        DateTime EndAt,
        BookingStatus Status);

}
