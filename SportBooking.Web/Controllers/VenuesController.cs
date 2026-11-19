using Microsoft.AspNetCore.Mvc;
using SportBooking.Application.Venues;
using SportBooking.Domain.Entities;
using SportBooking.Web.Models;

namespace SportBooking.Web.Controllers
{
    public class VenuesController : Controller
    {
        public readonly IVenueService _ctx;
        public VenuesController(IVenueService ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _ctx.GetAllAsync();

            var viewModel = dtos.Select(v => new VenuesViewModel
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                Phone = v.Phone,
                OpenTime = v.OpenTime,
                CloseTime = v.CloseTime,
                FieldCount = v.FieldCount,   // Chỉnh lại đúng tên property trong VenueDto
                MinPrice = v.MinPrice,       // Chỉnh lại đúng tên property trong VenueDto
                ImageUrl = v.ThumbnailUrl        // Chỉnh lại đúng tên property trong VenueDto
            }).ToList();

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetFieldsByVenue(int id)
        {
            var dto = await _ctx.GetDetailAsync(id);

            if (dto is null)
            {
                return NotFound();
            }

            ViewBag.VenueName = dto.Name;

            var viewModel = dto.Fields.Select(f => new FieldsViewModel
            {
                id = f.id,
                VenueName = f.VenueName,
                FieldName = f.Name,
                FieldType = f.FieldType,
                isActive = f.isActive,
                ImageUrl = f.ImageUrl
            }).ToList();

            return View(viewModel);
        }
        

        
    }
}
