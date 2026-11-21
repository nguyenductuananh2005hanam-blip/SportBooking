using System;
using System.Collections.Generic;
using System.Text;
using SportBooking.Application.Auth.Dtos;

namespace SportBooking.Application.Auth
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);

        Task<UserDto?> ValidateAsync(string email, string password, CancellationToken ct = default);

        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword,
        CancellationToken ct = default);

        Task<UserDto?> Profile(int userId, CancellationToken ct = default);
    }
}
