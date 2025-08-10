using SilafHotelManagementSystem.Models;
using System.Collections.Generic;

namespace SilafHotelManagementSystem.Services.Interfaces
{
    /// <summary>
    /// Synchronous service API the menu calls.
    /// </summary>
    public interface IHotelService
    {
        bool TryAddRoom(Room room, out string message);
        List<Room> GetAllRooms();

        bool TryReserveRoom(string guestName, int roomId, int nights, out string message);
        List<Booking> GetAllBookings();

        bool TryCancelBooking(int bookingId, out string message);

        Guest? FindGuestByName(string name);
        Guest? GetHighestPayingGuest();
    }
}
