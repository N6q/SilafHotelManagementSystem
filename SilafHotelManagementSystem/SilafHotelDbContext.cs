using SilafHotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace SilafHotelManagementSystem.Data
{
    public class SilafHotelDbContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(local);Initial Catalog=SilafHotelDB;Integrated Security=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Guest>()
                .HasMany(g => g.Bookings)
                .WithOne(b => b.Guest)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.Reviews)
                .WithOne(rv => rv.Room)
                .HasForeignKey(rv => rv.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Guest>()
                .HasMany(g => g.Reviews)
                .WithOne(rv => rv.Guest)
                .HasForeignKey(rv => rv.GuestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
