using Microsoft.AspNetCore.Mvc;
using SportBooking.Application.Auth;
using SportBooking.Application.Auth.Dtos;
using SportBooking.Application.Common.Exceptions;
using SportBooking.Web.Models;

namespace SportBooking.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService) => _authService = authService;

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _authService.RegisterAsync(
                    new RegisterDto(model.FullName, model.Email, model.Phone, model.Password));

                TempData["Success"] = "Đăng ký thành công, đăng nhập ngay.";
                return RedirectToAction("Login");
            }
            catch (Exception ex) when (ex is ConflictException or BusinessException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login() => View(new LoginViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var user = await _authService.ValidateAsync(model.Email, model.Password);
                if(user == null)
                {
                    TempData["Error"] = "Bạn chưa đăng nhập, vui lòng thử lại.";
                }
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
                HttpContext.Session.SetString("UserName", user.Fullname);
                TempData["Success"] = "Đăng nhập thành công";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) when (ex is ConflictException or BusinessException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
        public async Task<IActionResult> Profile(int id, ProfileViewModel model)
        {
            var profile = await _authService.Profile(id);
            var modelview = new ProfileViewModel
            {
                Id = profile.Id,
                FullName = profile.Fullname,
                Email = profile.Email,
                Phone = profile.PhoneNumber,
                Ngaytao = profile.CreatedAt.ToString("dd/MM/yyyy")
            };

            return View(modelview);
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
