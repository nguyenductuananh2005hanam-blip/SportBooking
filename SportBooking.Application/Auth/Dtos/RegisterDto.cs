using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Auth.Dtos
{
    public record RegisterDto(
        string Fullname,
        string Email,
        string PhoneNumber,
        string Password
        );
    
}
