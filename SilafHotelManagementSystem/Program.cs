using Microsoft.EntityFrameworkCore;
using SilafHotelManagementSystem.Data;                 // DbContext
using SilafHotelManagementSystem.Menus;               // AdminMenu
using SilafHotelManagementSystem.Repositories;        // (not used directly here; impls lives in Implementations)
using SilafHotelManagementSystem.Repositories.Implementations;
using SilafHotelManagementSystem.Repositories.Interfaces;
using SilafHotelManagementSystem.Services;            // HotelService (sync)
using SilafHotelManagementSystem.Services.Interfaces;

namespace SilafHotelManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create one DbContext for the whole app (simple console app)
            var context = new SilafHotelDbContext();

            try
            {
                // Ensure database schema is up to date (preferred over EnsureCreated when using migrations)
                context.Database.Migrate();

                // Build repos with the SAME DbContext instance
                IRoomRepository roomRepo = new RoomRepository(context);
                IGuestRepository guestRepo = new GuestRepository(context);
                IBookingRepository bookingRepo = new BookingRepository(context);
                IReviewRepository reviewRepo = new ReviewRepository(context);

                // Build a synchronous service that uses the repos (and thus the same context)
                IHotelService hotelService = new HotelService(roomRepo, guestRepo, bookingRepo, reviewRepo);

                // Start the menu (writes to DB + JSON as per your AdminMenu)
                var menu = new AdminMenu(hotelService);
                menu.Show();
            }
            finally
            {
                // Clean up the long-lived context when app exits
                context.Dispose();
            }
        }
    }
}
