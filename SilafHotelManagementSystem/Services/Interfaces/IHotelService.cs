using SilafHotelManagementSystem.Models;

namespace SilafHotelManagementSystem.Services.Interfaces
{
    public interface IHotelService
    {
        Task AddRoomAsync(Room room);
        Task<List<Room>> GetAllRoomsAsync();
        Task<List<Guest>> GetAllGuestsAsync();
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<Review>> GetAllReviewsAsync();

        Task ReserveRoomAsync(string guestName, int roomId, int nights);
        Task CancelBookingAsync(int bookingId);
        Task<Guest?> FindGuestByNameAsync(string name);
        Task<Guest?> GetHighestPayingGuestAsync();
    }
}
