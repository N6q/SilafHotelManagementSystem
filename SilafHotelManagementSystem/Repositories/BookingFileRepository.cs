using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    public class BookingFileRepository
    {
        public List<Booking> GetAll() => FileContext.LoadBookings();

        public Booking? GetById(int id) =>
            FileContext.LoadBookings().FirstOrDefault(b => b.BookingId == id);

        public void Add(Booking booking)
        {
            var bookings = FileContext.LoadBookings();
            bookings.Add(booking);
            FileContext.SaveBookings(bookings);
        }

        public void Update(Booking updatedBooking)
        {
            var bookings = FileContext.LoadBookings();
            var index = bookings.FindIndex(b => b.BookingId == updatedBooking.BookingId);
            if (index >= 0)
            {
                bookings[index] = updatedBooking;
                FileContext.SaveBookings(bookings);
            }
        }

        public void Delete(int id)
        {
            var bookings = FileContext.LoadBookings();
            var booking = bookings.FirstOrDefault(b => b.BookingId == id);
            if (booking != null)
            {
                bookings.Remove(booking);
                FileContext.SaveBookings(bookings);
            }
        }
    }
}
