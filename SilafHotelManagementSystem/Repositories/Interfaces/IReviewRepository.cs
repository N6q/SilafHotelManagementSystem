using SilafHotelManagementSystem.Models;

namespace SilafHotelManagementSystem.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
        Task<List<Review>> GetByGuestIdAsync(int guestId);
        Task<List<Review>> GetByRoomIdAsync(int roomId);
    }
}
