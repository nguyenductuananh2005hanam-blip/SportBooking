using Microsoft.AspNetCore.Mvc;
using SportBooking.Application.Fields;
using SportBooking.Web.Models;

namespace SportBooking.Web.Controllers
{
    public class FieldsController : Controller
    {
        public readonly IFieldService _ctx;

        public FieldsController(IFieldService ctx)
        {
            _ctx = ctx;
        }
        public async Task<IActionResult> Index(int id)
        {
            var model = await _ctx.GetAllAsync(id);
            if(model == null)
            {
                return NotFound();
            }
            var viewmodel = model.FieldPrices.Select(x => new FieldPricesViewModel
            {
                Id = x.Id,
                FieldId = x.FieldId,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Price = x.Price
            }).ToList();

            return View(viewmodel);
        }
    }
}
