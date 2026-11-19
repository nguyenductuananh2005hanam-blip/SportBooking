using SportBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }

        UserRole? Role { get; }

        bool IsAuthenticated { get; }
    }
}
