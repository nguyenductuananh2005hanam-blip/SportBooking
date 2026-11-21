using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Auth.Dtos
{
    public record UserDto(
        int Id,
        string Fullname,
        string Email,
        string PhoneNumber,
        DateTime CreatedAt,
        string Role
        );
    
}
