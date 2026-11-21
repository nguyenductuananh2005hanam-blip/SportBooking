using Microsoft.AspNetCore.Mvc;
using SportBooking.Application.Bookings;
using SportBooking.Application.Bookings.Dtos;
using SportBooking.Application.Common.Exceptions;
using SportBooking.Domain.Enums;
using SportBooking.Web.Models;

namespace SportBooking.Web.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Lấy thông tin đăng nhập từ Session hiện tại
        private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");
        private bool IsAdmin => string.Equals(
            HttpContext.Session.GetString("UserRole"),
            UserRole.Admin.ToString(),
            StringComparison.OrdinalIgnoreCase);

        // 1. Danh sách đơn đặt sân của người dùng hiện tại
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            if (CurrentUserId is null)
                return RedirectToLogin();

            var dtos = await _bookingService.GetMyBookingsAsync(CurrentUserId.Value, ct);
            var viewModel = dtos.Select(MapToViewModel).ToList();

            return View(viewModel);
        }

        // 2. Chi tiết một đơn đặt sân
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            if (CurrentUserId is null)
                return RedirectToLogin();

            var dto = await _bookingService.GetByIdAsync(id, CurrentUserId.Value, IsAdmin, ct);
            if (dto is null)
                return NotFound();

            return View(MapToViewModel(dto));
        }

        // 3. Hiển thị form đặt sân (khớp với link asp-controller="Bookings" asp-action="Create" asp-route-fieldId="...")
        [HttpGet]
        public async Task<IActionResult> Create(int fieldId, DateOnly? date, CancellationToken ct)
        {
            if (CurrentUserId is null)
                return RedirectToLogin();

            var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var bookedSlots = await _bookingService.GetBookedSlotsAsync(fieldId, selectedDate, ct);

            var viewModel = new CreateBookingViewModel
            {
                FieldId = fieldId,
                BookingDate = selectedDate,
                BookedSlots = bookedSlots.Select(s => new BookedSlotViewModel
                {
                    BookingId = s.BookingId,
                    StartAt = s.StartAt,
                    EndAt = s.EndAt,
                    Status = s.Status
                }).ToList()
            };

            return View(viewModel);
        }

        // 4. Xử lý submit đặt sân
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            if (CurrentUserId is null)
                return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                await PopulateBookedSlotsAsync(model, ct);
                return View(model);
            }

            try
            {
                var dto = new CreateBookingDto(
                    FieldId: model.FieldId,
                    StartAt: model.BookingDate.ToDateTime(model.StartTime),
                    EndAt: model.BookingDate.ToDateTime(model.EndTime),
                    Note: model.Note);

                var created = await _bookingService.CreateAsync(CurrentUserId.Value, dto, ct);

                TempData["Success"] = "Đặt sân thành công!";
                return RedirectToAction(nameof(Details), new { id = created.Id });
            }
            catch (Exception ex) when (ex is BusinessException or ConflictException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateBookedSlotsAsync(model, ct);
                return View(model);
            }
        }

        // 5. Endpoint JSON cho JS gọi khi người dùng đổi ngày trên form đặt sân
        [HttpGet]
        public async Task<IActionResult> BookedSlots(int fieldId, DateOnly date, CancellationToken ct)
        {
            var slots = await _bookingService.GetBookedSlotsAsync(fieldId, date, ct);
            return Json(slots.Select(s => new
            {
                start = s.StartAt.ToString("HH:mm"),
                end = s.EndAt.ToString("HH:mm"),
                status = s.Status.ToString()
            }));
        }

        // 6. Hủy đơn đặt sân
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            if (CurrentUserId is null)
                return RedirectToLogin();

            try
            {
                await _bookingService.CancelAsync(id, CurrentUserId.Value, IsAdmin, ct);
                TempData["Success"] = "Đã hủy đơn đặt sân thành công.";
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // 7. Cập nhật trạng thái đơn (chỉ dành cho Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, BookingStatus status, CancellationToken ct)
        {
            if (CurrentUserId is null)
                return RedirectToLogin();

            if (!IsAdmin)
                return Forbid();

            try
            {
                await _bookingService.UpdateStatusAsync(id, status, ct);
                TempData["Success"] = "Cập nhật trạng thái đơn thành công.";
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private IActionResult RedirectToLogin()
        {
            TempData["Error"] = "Vui lòng đăng nhập để sử dụng chức năng đặt sân.";
            return RedirectToAction("Login", "Account");
        }

        private async Task PopulateBookedSlotsAsync(CreateBookingViewModel model, CancellationToken ct)
        {
            var slots = await _bookingService.GetBookedSlotsAsync(model.FieldId, model.BookingDate, ct);
            model.BookedSlots = slots.Select(s => new BookedSlotViewModel
            {
                BookingId = s.BookingId,
                StartAt = s.StartAt,
                EndAt = s.EndAt,
                Status = s.Status
            }).ToList();
        }

        private static BookingItemViewModel MapToViewModel(BookingDto dto) => new()
        {
            Id = dto.Id,
            UserId = dto.UserId,
            UserName = dto.UserName,
            UserPhone = dto.UserPhone,
            FieldId = dto.FieldId,
            FieldName = dto.FieldName,
            VenueName = dto.VenueName,
            VenueAddress = dto.VenueAddress,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            TotalPrice = dto.TotalPrice,
            Status = dto.Status,
            Note = dto.Note,
            CreatedAt = dto.CreatedAt
        };

        public async Task<IActionResult> GetBookingHistoried(CancellationToken ct)
        {
            if (CurrentUserId == null)
            {
                TempData["Errorr"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account");
            }
            var historied = await _bookingService.GetMyBookingsAsync(CurrentUserId.Value, ct);

            var viewmodel = historied.Select(f => new BookingHistoriedViewModel
            {
                Id = f.Id,
                FieldName = f.FieldName,
                StartAt = f.StartAt,
                EndAt = f.EndAt,
                TotalPrice = f.TotalPrice,
                Status = f.Status,
                Note = f.Note,
                CreatedAt = f.CreatedAt.ToString("dd/MM/yyyy")
            });
            return View(viewmodel);
        }
    }
}
