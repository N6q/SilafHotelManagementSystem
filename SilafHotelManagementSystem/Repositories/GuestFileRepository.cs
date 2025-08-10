using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    /// <summary>
    /// Simple JSON-backed repository for <see cref="Guest"/> records.
    /// Uses <see cref="FileContext"/> to load/save the entire collection each call.
    /// </summary>
    public class GuestFileRepository
    {
        /// <summary>
        /// Returns all guests from the JSON file.
        /// </summary>
        public List<Guest> GetAll() => FileContext.LoadGuests();

        /// <summary>
        /// Returns a single guest by its identifier, or null if not found.
        /// </summary>
        /// <param name="id">GuestId to search for.</param>
        public Guest? GetById(int id) =>
            FileContext.LoadGuests().FirstOrDefault(g => g.GuestId == id);

        /// <summary>
        /// Appends a new guest to the list and saves the file.
        /// </summary>
        /// <param name="guest">The guest to add (caller assigns GuestId).</param>
        public void Add(Guest guest)
        {
            // Load current snapshot from disk
            var guests = FileContext.LoadGuests();

            // Add the new item (no duplicate check here)
            guests.Add(guest);

            // Persist updated list to disk
            FileContext.SaveGuests(guests);
        }

        /// <summary>
        /// Replaces an existing guest (matched by GuestId) and saves the file.
        /// If the guest is not found, this is a no-op.
        /// </summary>
        /// <param name="updatedGuest">Guest with same GuestId and new values.</param>
        public void Update(Guest updatedGuest)
        {
            // Load current snapshot
            var guests = FileContext.LoadGuests();

            // Find the index for the matching GuestId
            var index = guests.FindIndex(g => g.GuestId == updatedGuest.GuestId);

            // If found, replace and persist
            if (index >= 0)
            {
                guests[index] = updatedGuest;
                FileContext.SaveGuests(guests);
            }
            // else: silently ignore to keep API simple
        }

        /// <summary>
        /// Removes a guest by id and saves the file.
        /// If the guest is not found, nothing happens.
        /// </summary>
        /// <param name="id">GuestId to remove.</param>
        public void Delete(int id)
        {
            // Load current snapshot
            var guests = FileContext.LoadGuests();

            // Find the target guest
            var guest = guests.FirstOrDefault(g => g.GuestId == id);

            // If present, remove and persist
            if (guest != null)
            {
                guests.Remove(guest);
                FileContext.SaveGuests(guests);
            }
            // else: no-op
        }
    }
}
