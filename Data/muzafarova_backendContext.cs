using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using muzafarova_backend.Models;

namespace muzafarova_backend.Data
{
    public class muzafarova_backendContext : DbContext
    {
        public muzafarova_backendContext (DbContextOptions<muzafarova_backendContext> options)
            : base(options)
        {
        }

        public DbSet<muzafarova_backend.Models.flight> Flight { get; set; } = default!;
        public DbSet<muzafarova_backend.Models.passenger> Passenger { get; set; } = default!;
        public DbSet<muzafarova_backend.Models.booking> Booking { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь между booking и passenger
            modelBuilder.Entity<booking>()
                .HasOne(b => b.Passenger)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PassengerId)
                .OnDelete(DeleteBehavior.NoAction); // Изменено на Restrict для предотвращения каскадного удаления

            // Связь между booking и flight
            modelBuilder.Entity<booking>()
                .HasOne(b => b.Flight)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.NoAction);
                //.OnUpdate(UpdateBehavior.NoAction); // Оставлено каскадное удаление
        }

    }
}
