using SilafHotelManagementSystem.Data;                 // DbContext for database creation
using SilafHotelManagementSystem.Menus;               // AdminMenu (sync Show)
using SilafHotelManagementSystem.Services;            // HotelService (sync service)
using SilafHotelManagementSystem.Services.Interfaces; // IHotelService

namespace SilafHotelManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create a DbContext just to ensure the database exists
            var context = new SilafHotelDbContext();
            try
            {
                // Create DB and tables if they don't exist
                context.Database.EnsureCreated();
            }
            finally
            {
                // Close the context used for setup
                context.Dispose();
            }

            // Build the hotel service (sync version that uses DbContext internally per call)
            IHotelService hotelService = new HotelService();

            // Start the admin menu (sync; saves/reads to BOTH DB and JSON as implemented in AdminMenu)
            var menu = new AdminMenu(hotelService);
            menu.Show();
        }
    }
}
