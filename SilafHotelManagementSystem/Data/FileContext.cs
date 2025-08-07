using SilafHotelManagementSystem.Models;
using System.Text.Json;

namespace SilafHotelManagementSystem.Data
{
    public static class FileContext
    {
        private static string BasePath = "DataFiles";

        static FileContext()
        {
            Directory.CreateDirectory(BasePath);
        }

        public static void SaveRooms(List<Room> rooms)
        {
            string json = JsonSerializer.Serialize(rooms);
            File.WriteAllText(Path.Combine(BasePath, "rooms.json"), json);
        }

        public static List<Room> LoadRooms()
        {
            string path = Path.Combine(BasePath, "rooms.json");
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Room>>(File.ReadAllText(path)) ?? new List<Room>()
                : new List<Room>();
        }

        public static void SaveGuests(List<Guest> guests)
        {
            string json = JsonSerializer.Serialize(guests);
            File.WriteAllText(Path.Combine(BasePath, "guests.json"), json);
        }

        public static List<Guest> LoadGuests()
        {
            string path = Path.Combine(BasePath, "guests.json");
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Guest>>(File.ReadAllText(path)) ?? new List<Guest>()
                : new List<Guest>();
        }

        public static void SaveBookings(List<Booking> bookings)
        {
            string json = JsonSerializer.Serialize(bookings);
            File.WriteAllText(Path.Combine(BasePath, "bookings.json"), json);
        }

        public static List<Booking> LoadBookings()
        {
            string path = Path.Combine(BasePath, "bookings.json");
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Booking>>(File.ReadAllText(path)) ?? new List<Booking>()
                : new List<Booking>();
        }

        public static void SaveReviews(List<Review> reviews)
        {
            string json = JsonSerializer.Serialize(reviews);
            File.WriteAllText(Path.Combine(BasePath, "reviews.json"), json);
        }

        public static List<Review> LoadReviews()
        {
            string path = Path.Combine(BasePath, "reviews.json");
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Review>>(File.ReadAllText(path)) ?? new List<Review>()
                : new List<Review>();
        }
    }
}
