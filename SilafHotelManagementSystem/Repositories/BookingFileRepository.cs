using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    /// <summary>
    /// Simple JSON-backed repository for <see cref="Booking"/> records.
    /// Uses <see cref="FileContext"/> to read/write the entire list each time.
    /// </summary>
    public class BookingFileRepository
    {
        /// <summary>
        /// Returns all bookings from the JSON file.
        /// </summary>
        public List<Booking> GetAll() => FileContext.LoadBookings();

        /// <summary>
        /// Returns a single booking by its identifier, or null if not found.
        /// </summary>
        /// <param name="id">BookingId to search for.</param>
        public Booking? GetById(int id) =>
            FileContext.LoadBookings().FirstOrDefault(b => b.BookingId == id);

        /// <summary>
        /// Appends a new booking to the list and saves the file.
        /// </summary>
        /// <param name="booking">The booking to add.</param>
        public void Add(Booking booking)
        {
            // load current state
            var bookings = FileContext.LoadBookings();

            // add the new item (assumes BookingId already assigned by caller)
            bookings.Add(booking);

            // persist to disk
            FileContext.SaveBookings(bookings);
        }

        /// <summary>
        /// Replaces an existing booking (matched by BookingId) and saves the file.
        /// If the booking is not found, nothing happens.
        /// </summary>
        /// <param name="updatedBooking">A booking with the same BookingId and new values.</param>
        public void Update(Booking updatedBooking)
        {
            // load current state
            var bookings = FileContext.LoadBookings();

            // find index of the booking we want to update
            var index = bookings.FindIndex(b => b.BookingId == updatedBooking.BookingId);

            // if present, replace and save
            if (index >= 0)
            {
                bookings[index] = updatedBooking;
                FileContext.SaveBookings(bookings);
            }
            // else: silently ignore (no-op) to keep behavior simple
        }

        /// <summary>
        /// Removes a booking by id and saves the file.
        /// If the booking is not found, nothing happens.
        /// </summary>
        /// <param name="id">BookingId to remove.</param>
        public void Delete(int id)
        {
            // load current state
            var bookings = FileContext.LoadBookings();

            // find the booking to remove
            var booking = bookings.FirstOrDefault(b => b.BookingId == id);

            // if found, remove and persist
            if (booking != null)
            {
                bookings.Remove(booking);
                FileContext.SaveBookings(bookings);
            }
            // else: no-op
        }
    }
}
