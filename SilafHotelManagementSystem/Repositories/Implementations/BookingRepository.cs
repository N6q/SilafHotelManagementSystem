using Microsoft.EntityFrameworkCore;
using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories.Implementations
{
    /// <summary>
    /// EF Core repository for Booking entities (synchronous).
    /// </summary>
    public class BookingRepository : IBookingRepository
    {
        private readonly SilafHotelDbContext _context;

        public BookingRepository(SilafHotelDbContext context)
        {
            _context = context;
        }

        /// <summary>Return all bookings with related Guest and Room (read-only).</summary>
        public List<Booking> GetAll()
        {
            return _context.Bookings
                           .Include(b => b.Guest)
                           .Include(b => b.Room)
                           .AsNoTracking()
                           .ToList();
        }

        /// <summary>Find one booking by id including Guest and Room; null if not found.</summary>
        public Booking? GetById(int id)
        {
            return _context.Bookings
                           .Include(b => b.Guest)
                           .Include(b => b.Room)
                           .AsNoTracking()
                           .FirstOrDefault(b => b.BookingId == id);
        }

        /// <summary>Insert a new booking and save changes.</summary>
        public void Add(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        /// <summary>Update an existing booking and save changes.</summary>
        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();
        }

        /// <summary>Delete a booking by id (no-op if not found).</summary>
        public void Delete(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }
        }

        /// <summary>Return all bookings for a given guest id (includes Room).</summary>
        public List<Booking> GetByGuestId(int guestId)
        {
            return _context.Bookings
                           .Where(b => b.GuestId == guestId)
                           .Include(b => b.Room)
                           .AsNoTracking()
                           .ToList();
        }

        /// <summary>Return all bookings for a given room id (includes Guest).</summary>
        public List<Booking> GetByRoomId(int roomId)
        {
            return _context.Bookings
                           .Where(b => b.RoomId == roomId)
                           .Include(b => b.Guest)
                           .AsNoTracking()
                           .ToList();
        }
    }
}
