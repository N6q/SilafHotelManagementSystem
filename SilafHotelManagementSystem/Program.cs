using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Repositories;
using SilafHotelManagementSystem.Repositories.Implementations;
using SilafHotelManagementSystem.Repositories.Interfaces;
using SilafHotelManagementSystem.Services;
using SilafHotelManagementSystem.Services.Interfaces;
using SilafHotelManagementSystem.Menus;

namespace SilafHotelManagementSystem
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Ensure DB is created
            using SilafHotelDbContext context = new SilafHotelDbContext();
            context.Database.EnsureCreated();

            // Instantiate repositories
            IRoomRepository roomRepo = new RoomRepository(context);
            IGuestRepository guestRepo = new GuestRepository(context);
            IBookingRepository bookingRepo = new BookingRepository(context);
            IReviewRepository reviewRepo = new ReviewRepository(context);

            // Instantiate service
            IHotelService hotelService = new HotelService(
                roomRepo, guestRepo, bookingRepo, reviewRepo
            );

            // Launch admin menu
            AdminMenu menu = new AdminMenu(hotelService);
            await menu.ShowAsync();
        }
    }
}
