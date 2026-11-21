using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SportBooking.Application.Auth;
using SportBooking.Application.Venues;
using SportBooking.Application.Common.Interfaces;
using SportBooking.Infrastructure.Data;
using SportBooking.Infrastructure.Services;
using SportBooking.Application.Fields;
using SportBooking.Application.Bookings;
using SportBooking.Domain.Entities;
using SportBooking.Application.Payments;

var builder = WebApplication.CreateBuilder(args);

// ----- MVC -----
builder.Services.AddControllersWithViews();

// ----- Database -----
builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

// ----- Services -----
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IImageStorageService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new ImageStorageService(env.WebRootPath);
});

// Bật thêm khi đã viết các service tương ứng:
// builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();   // using SportBooking.Web.Services;
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// ----- Đăng nhập bằng cookie (cần cho Login, [Authorize]) -----
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/AccessDenied";
        o.ExpireTimeSpan = TimeSpan.FromDays(7);
        o.SlidingExpiration = true;
        o.Cookie.HttpOnly = true;
    });

// Chỉ bật Session nếu thật sự dùng HttpContext.Session. Khi đó phải có CẢ HAI dòng này:
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();   // chỉ bật cùng với AddSession ở trên

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // phải đứng trước UseAuthorization
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();