using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace SilafHotelManagementSystem.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly SilafHotelDbContext _context;

        public ReviewRepository(SilafHotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Review>> GetByGuestIdAsync(int guestId)
        {
            return await _context.Reviews
                .Where(r => r.GuestId == guestId)
                .Include(r => r.Room)
                .ToListAsync();
        }

        public async Task<List<Review>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Reviews
                .Where(r => r.RoomId == roomId)
                .Include(r => r.Guest)
                .ToListAsync();
        }
    }
}
