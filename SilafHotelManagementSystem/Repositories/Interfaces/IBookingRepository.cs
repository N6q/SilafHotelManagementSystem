using SilafHotelManagementSystem.Models;

namespace SilafHotelManagementSystem.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<List<Booking>> GetByGuestIdAsync(int guestId);
        Task<List<Booking>> GetByRoomIdAsync(int roomId);
    }
}
