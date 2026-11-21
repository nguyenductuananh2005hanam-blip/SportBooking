using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportBooking.Application.Bookings;
using SportBooking.Application.Bookings.Dtos;
using SportBooking.Application.Common.Exceptions;
using SportBooking.Application.Payments;
using SportBooking.Application.Payments.Dtos;
using SportBooking.Domain.Enums;
using SportBooking.Web.Models;

namespace SportBooking.Web.Controllers;

public class PaymentsController : Controller
{
    private readonly IPaymentService _payments;
    private readonly IBookingService _bookings;

    public PaymentsController(IPaymentService payments, IBookingService bookings)
    {
        _payments = payments;
        _bookings = bookings;
    }
    private int ?CurrentUserId => HttpContext.Session.GetInt32("UserId");
    private bool IsAdmin => string.Equals(
        HttpContext.Session.GetString("UserRole"),
        UserRole.Admin.ToString(),
        StringComparison.OrdinalIgnoreCase);
    // GET /Payments/Checkout?bookingId=5
    [HttpGet]
    public async Task<IActionResult> Checkout(int bookingId, CancellationToken ct)
    {
        BookingDto booking;
        
        try
        {
            booking = await _bookings.GetByIdAsync(bookingId, CurrentUserId.Value, IsAdmin, ct);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        if (booking.Status != BookingStatus.Pending)
        {
            TempData["Error"] = "Đơn đặt sân này không còn ở trạng thái chờ thanh toán.";
            return RedirectToAction("MyBookings", "Bookings");
        }

        return View(BuildViewModel(booking, method: null));
    }

    // POST /Payments/Checkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(PaymentViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return await ReloadFormAsync(vm, ct);

        try
        {
            var payment = await _payments.CreatePaymentAsync(
                new CreatePaymentDto(vm.BookingId, vm.Method!.Value), ct);

            return RedirectToAction(nameof(Details), new { id = payment.Id });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return await ReloadFormAsync(vm, ct);
        }
    }

    // GET /Payments/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        try
        {
            return View(await _payments.GetByIdAsync(id, ct));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
    

    private async Task<IActionResult> ReloadFormAsync(PaymentViewModel vm, CancellationToken ct)
    {
        try
        {
            var booking = await _bookings.GetByIdAsync(vm.BookingId,CurrentUserId.Value, IsAdmin,ct);
            return View(BuildViewModel(booking, vm.Method));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    private static PaymentViewModel BuildViewModel(BookingDto booking, PaymentMethod? method) => new()
    {
        BookingId = booking.Id,
        Method = method,
        FieldName = booking.FieldName,
        VenueName = booking.VenueName,
        StartAt = booking.StartAt,
        EndAt = booking.EndAt,
        Amount = booking.TotalPrice
    };
}