using Microsoft.EntityFrameworkCore;
using SportBooking.Application.Common.Interfaces;
using SportBooking.Domain.Entities;
using SportBooking.Domain.Enums;

namespace SportBooking.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext , IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldPrice> FieldPrices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ----- User -----
            b.Entity<User>(e =>
            {
                e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
                e.Property(x => x.Email).HasMaxLength(150).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(20).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
                e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
                e.HasIndex(x => x.Email).IsUnique();
                e.HasIndex(x => x.Phone).IsUnique();
            });

            // ----- Venue -----
            b.Entity<Venue>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Address).HasMaxLength(255).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(20);

                e.HasOne(x => x.Owner)
                 .WithMany(u => u.Venues)
                 .HasForeignKey(x => x.OwnerId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ----- Field -----
            b.Entity<Field>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.FieldType).HasConversion<byte>();
                e.Property(x => x.ImageUrl).HasMaxLength(500);
                e.HasOne(x => x.Venue)
                 .WithMany(v => v.Fields)
                 .HasForeignKey(x => x.VenueId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ----- FieldPrice -----
            b.Entity<FieldPrice>(e =>
            {
                e.Property(x => x.DayType).HasConversion<string>().HasMaxLength(10);
                e.Property(x => x.Price).HasPrecision(12, 0);

                e.HasOne(x => x.Field)
                 .WithMany(f => f.Prices)
                 .HasForeignKey(x => x.FieldId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ----- Booking -----
            b.Entity<Booking>(e =>
            {
                e.Property(x => x.TotalPrice).HasPrecision(12, 0);
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(x => x.Note).HasMaxLength(500);

                e.HasOne(x => x.User)
                 .WithMany(u => u.Bookings)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Field)
                 .WithMany(f => f.Bookings)
                 .HasForeignKey(x => x.FieldId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.FieldId, x.StartAt, x.EndAt });

                e.ToTable(t => t.HasCheckConstraint("CK_Bookings_EndAfterStart", "[EndAt] > [StartAt]"));
            });

            // ----- Payment -----
            b.Entity<Payment>(e =>
            {
                e.Property(x => x.Amount).HasPrecision(12, 0);
                e.Property(x => x.Method).HasConversion<string>().HasMaxLength(20);
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

                e.HasOne(x => x.Booking)
                 .WithMany(bk => bk.Payments)
                 .HasForeignKey(x => x.BookingId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}