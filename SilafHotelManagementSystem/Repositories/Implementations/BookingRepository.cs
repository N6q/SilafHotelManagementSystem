
using Microsoft.EntityFrameworkCore;
using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;

namespace SilafHotelManagementSystem.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly SilafHotelDbContext _context;

        public BookingRepository(SilafHotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Booking>> GetByGuestIdAsync(int guestId)
        {
            return await _context.Bookings
                .Where(b => b.GuestId == guestId)
                .Include(b => b.Room)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId)
                .Include(b => b.Guest)
                .ToListAsync();
        }
    }
}
