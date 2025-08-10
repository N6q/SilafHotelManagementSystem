using SilafHotelManagementSystem.Models;
using System.Collections.Generic;

namespace SilafHotelManagementSystem.Repositories.Interfaces
{
    /// <summary>
    /// Synchronous repository contract for Booking entities.
    /// </summary>
    public interface IBookingRepository
    {
        List<Booking> GetAll();           // Include Guest + Room where useful
        Booking? GetById(int id);         // Include Guest + Room
        void Add(Booking booking);
        void Update(Booking booking);
        void Delete(int id);

        List<Booking> GetByGuestId(int guestId); // All bookings for a guest
        List<Booking> GetByRoomId(int roomId);   // All bookings for a room
    }
}
