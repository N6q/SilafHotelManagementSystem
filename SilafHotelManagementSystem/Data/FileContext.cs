using SilafHotelManagementSystem.Models;
using System.Text.Json;

namespace SilafHotelManagementSystem.Data
{
    /// <summary>
    /// Simple JSON-based persistence utility.
    /// Stores and retrieves Rooms, Guests, Bookings, and Reviews
    /// in separate files under the DataFiles folder.
    /// </summary>
    public static class FileContext
    {
        /// <summary>
        /// Root folder where all JSON files are stored.
        /// </summary>
        private static string BasePath = "DataFiles";

        /// <summary>
        /// Static constructor runs once to ensure the storage folder exists.
        /// </summary>
        static FileContext()
        {
            // Create the folder if it doesn't exist (no-op if it already exists)
            Directory.CreateDirectory(BasePath);
        }

        /// <summary>
        /// Overwrites rooms.json with the provided list.
        /// </summary>
        /// <param name="rooms">All rooms to store.</param>
        public static void SaveRooms(List<Room> rooms)
        {
            // Serialize the list to JSON (compact by default)
            string json = JsonSerializer.Serialize(rooms);

            // Write the JSON to DataFiles/rooms.json
            File.WriteAllText(Path.Combine(BasePath, "rooms.json"), json);
        }

        /// <summary>
        /// Loads rooms from rooms.json if it exists; otherwise returns an empty list.
        /// </summary>
        public static List<Room> LoadRooms()
        {
            // Full path to the rooms file
            string path = Path.Combine(BasePath, "rooms.json");

            // If the file exists, read and deserialize; otherwise return a new empty list.
            // If deserialization returns null, fall back to an empty list.
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Room>>(File.ReadAllText(path)) ?? new List<Room>()
                : new List<Room>();
        }

        /// <summary>
        /// Overwrites guests.json with the provided list.
        /// </summary>
        /// <param name="guests">All guests to store.</param>
        public static void SaveGuests(List<Guest> guests)
        {
            // Serialize and write to DataFiles/guests.json
            string json = JsonSerializer.Serialize(guests);
            File.WriteAllText(Path.Combine(BasePath, "guests.json"), json);
        }

        /// <summary>
        /// Loads guests from guests.json if it exists; otherwise returns an empty list.
        /// </summary>
        public static List<Guest> LoadGuests()
        {
            string path = Path.Combine(BasePath, "guests.json");

            // Safe load with empty fallback
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Guest>>(File.ReadAllText(path)) ?? new List<Guest>()
                : new List<Guest>();
        }

        /// <summary>
        /// Overwrites bookings.json with the provided list.
        /// </summary>
        /// <param name="bookings">All bookings to store.</param>
        public static void SaveBookings(List<Booking> bookings)
        {
            // Serialize and write to DataFiles/bookings.json
            string json = JsonSerializer.Serialize(bookings);
            File.WriteAllText(Path.Combine(BasePath, "bookings.json"), json);
        }

        /// <summary>
        /// Loads bookings from bookings.json if it exists; otherwise returns an empty list.
        /// </summary>
        public static List<Booking> LoadBookings()
        {
            string path = Path.Combine(BasePath, "bookings.json");

            // Safe load with empty fallback
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Booking>>(File.ReadAllText(path)) ?? new List<Booking>()
                : new List<Booking>();
        }

        /// <summary>
        /// Overwrites reviews.json with the provided list.
        /// </summary>
        /// <param name="reviews">All reviews to store.</param>
        public static void SaveReviews(List<Review> reviews)
        {
            // Serialize and write to DataFiles/reviews.json
            string json = JsonSerializer.Serialize(reviews);
            File.WriteAllText(Path.Combine(BasePath, "reviews.json"), json);
        }

        /// <summary>
        /// Loads reviews from reviews.json if it exists; otherwise returns an empty list.
        /// </summary>
        public static List<Review> LoadReviews()
        {
            string path = Path.Combine(BasePath, "reviews.json");

            // Safe load with empty fallback
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Review>>(File.ReadAllText(path)) ?? new List<Review>()
                : new List<Review>();
        }
    }
}
