using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SportBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Venue> Venues { get; }
        DbSet<Field> Fields { get; }
        DbSet<FieldPrice> FieldPrices { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<Payment> Payments { get; }

        /// <summary>
        /// Dùng để mở transaction, ví dụ khi kiểm tra trùng giờ và tạo đơn đặt sân
        /// (Database.BeginTransactionAsync(IsolationLevel.Serializable)).
        /// </summary>
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
