using SilafHotelManagementSystem.Models;

namespace SilafHotelManagementSystem.Repositories.Interfaces
{
    public interface IGuestRepository
    {
        Task<List<Guest>> GetAllAsync();
        Task<Guest?> GetByIdAsync(int id);
        Task AddAsync(Guest guest);
        Task UpdateAsync(Guest guest);
        Task DeleteAsync(int id);
    }
}
