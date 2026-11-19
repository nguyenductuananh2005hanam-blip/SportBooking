using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SportBooking.Application.Auth.Dtos;
using SportBooking.Application.Common.Exceptions;
using SportBooking.Application.Common.Interfaces;
using SportBooking.Domain.Entities;
using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace SportBooking.Application.Auth
{
    public class AuthService : IAuthService
    {
        public const int minLengtPass = 8;

        public readonly IApplicationDbContext _context;
        public readonly IPasswordHasher _passhash;

        public AuthService(IApplicationDbContext context, IPasswordHasher passhash)
        {
            _context = context;
            _passhash = passhash;
        }
        private static void EnsurePasswordIsValid(string? password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < minLengtPass)
                throw new BusinessException($"Mật khẩu phải có ít nhất {minLengtPass} ký tự");
        }
        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, ct) ?? throw new NotFoundException("Không tìm thấy tài khoản");

            if (!_passhash.Verify(currentPassword, user.PasswordHash))
                throw new BusinessException("Mật khẩu hiện tại không đúng");

            EnsurePasswordIsValid(newPassword);

            user.PasswordHash = _passhash.Hash(newPassword);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            var fullName = dto.Fullname?.Trim() ?? string.Empty;
            var email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            var phone = dto.PhoneNumber?.Trim() ?? string.Empty;

            if (fullName.Length == 0 || email.Length == 0 || phone.Length == 0)
                throw new BusinessException("Vui lòng nhập đầy đủ họ tên, email và số điện thoại");

            EnsurePasswordIsValid(dto.Password);

            if (await _context.Users.AnyAsync(u => u.Email == email, ct))
                throw new ConflictException("Email đã được sử dụng");

            if (await _context.Users.AnyAsync(u => u.Phone == phone, ct))
                throw new ConflictException("Số điện thoại đã được sử dụng");

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                PasswordHash = _passhash.Hash(dto.Password),
                Role = UserRole.Customer        
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException )
            {
                //var realError = ex.InnerException?.Message ?? ex.Message;
                //throw new Exception($"Lỗi thực sự từ DB là: {realError}");
               throw new ConflictException("Đăng ký không thành công");

            }

            return ToDto(user);

        }

        public async Task<UserDto?> ValidateAsync(string email, string password, CancellationToken ct = default)
        {
            var mail = email?.Trim().ToLowerInvariant() ?? string.Empty;
            var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == mail, ct);

            if (user is null || !_passhash.Verify(password, user.PasswordHash))
                throw new BusinessException("Email hoặc mật khẩu không chính xác");

            return ToDto(user);
        }
        private static UserDto ToDto(User u) => new(u.Id, u.FullName, u.Email, u.Phone, u.Role.ToString());
    }
}
