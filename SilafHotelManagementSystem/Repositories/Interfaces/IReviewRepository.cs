using SilafHotelManagementSystem.Models;
using System.Collections.Generic;

namespace SilafHotelManagementSystem.Repositories.Interfaces
{
    /// <summary>
    /// Synchronous repository contract for Review entities.
    /// </summary>
    public interface IReviewRepository
    {
        List<Review> GetAll();          // Include Guest + Room where useful
        Review? GetById(int id);        // Include Guest + Room
        void Add(Review review);
        void Update(Review review);
        void Delete(int id);

        List<Review> GetByGuestId(int guestId); // All reviews for a guest
        List<Review> GetByRoomId(int roomId);   // All reviews for a room
    }
}
