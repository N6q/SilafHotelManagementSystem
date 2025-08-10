using SilafHotelManagementSystem.Data;                   // DbContext for direct DB access where needed
using SilafHotelManagementSystem.Models;                // Room, Guest, Booking, Review models
using SilafHotelManagementSystem.Repositories;          // JSON file repositories
using SilafHotelManagementSystem.Services.Interfaces;   // IHotelService (sync Try... methods)
using System;
using System.Linq;

namespace SilafHotelManagementSystem.Menus
{
    public class AdminMenu
    {
        // Service used for all database operations (rooms, bookings, guests)
        private readonly IHotelService _hotelService;

        // File repositories for JSON mirroring (rooms, guests, bookings)
        private readonly RoomFileRepository _roomFileRepo = new RoomFileRepository();
        private readonly GuestFileRepository _guestFileRepo = new GuestFileRepository();
        private readonly BookingFileRepository _bookingFileRepo = new BookingFileRepository();

        // Simple cap for input validation
        private const int MaxNameLength = 100;

        // Constructor receives the service dependency
        public AdminMenu(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // Main loop: show menu, read choice, execute action
        public void Show()
        {
            Console.Clear(); // clear screen at start

            while (true)
            {
                // draw menu
                Console.Clear();
                Console.WriteLine("==== Silaf Hotel Management System ====");
                Console.WriteLine("1. Add New Room");
                Console.WriteLine("2. View All Rooms");
                Console.WriteLine("3. Make a Reservation");
                Console.WriteLine("4. View All Bookings");
                Console.WriteLine("5. Cancel a Booking");
                Console.WriteLine("6. Find Guest by Name");
                Console.WriteLine("7. Show Highest-Paying Guest");
                Console.WriteLine("8. Add Review");
                Console.WriteLine("9. View All Reviews");
                Console.WriteLine("0. Exit");

                // read and validate menu choice
                string choice;
                while (true)
                {
                    Console.Write("Select an option (0-9): ");
                    choice = Console.ReadLine()?.Trim();
                    if (choice is "0" or "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9") break;
                    Console.WriteLine("❌ Invalid option.");
                }

                // execute selected action with basic exception safety
                try
                {
                    switch (choice)
                    {
                        case "1": AddRoom(); break;
                        case "2": ViewAllRooms(); break;
                        case "3": MakeReservation(); break;
                        case "4": ViewAllBookings(); break;
                        case "5": CancelBooking(); break;
                        case "6": FindGuest(); break;
                        case "7": ShowTopGuest(); break;
                        case "8": AddReview(); break;
                        case "9": ViewAllReviews(); break;
                        case "0":
                            Console.WriteLine("Exiting...");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    // catch-all to avoid crashing the app; message shown to user
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }

                // pause before returning to menu
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }

        // Add a room to DB, then mirror the same room to JSON
        private void AddRoom()
        {
            // read and validate room number
            Console.Write("\nEnter Room Number: ");
            if (!int.TryParse(Console.ReadLine(), out int number) || number <= 0)
            {
                Console.WriteLine("❌ Invalid room number.");
                return;
            }

            // read and validate daily rate
            Console.Write("Enter Daily Rate: ");
            if (!double.TryParse(Console.ReadLine(), out double rate) || rate <= 0)
            {
                Console.WriteLine("❌ Invalid daily rate.");
                return;
            }

            // attempt to add to DB via service
            var dbOk = _hotelService.TryAddRoom(new Room
            {
                RoomNumber = number,
                DailyRate = rate,
                IsReserved = false
            }, out var dbMsg);

            // show DB result
            Console.WriteLine(dbOk ? "✅ DB: " + dbMsg : "❌ DB: " + dbMsg);
            if (!dbOk) return; // stop if DB insert failed

            // fetch the DB room to get its assigned RoomId
            var dbRoom = _hotelService.GetAllRooms().FirstOrDefault(r => r.RoomNumber == number);
            if (dbRoom == null)
            {
                Console.WriteLine("❌ Could not fetch room from DB after add.");
                return;
            }

            // mirror to JSON: either insert new record or update existing one
            var rooms = _roomFileRepo.GetAll();
            var fileRoom = rooms.FirstOrDefault(r => r.RoomNumber == number);

            if (fileRoom == null)
            {
                // insert new JSON room aligned with DB RoomId
                _roomFileRepo.Add(new Room
                {
                    RoomId = dbRoom.RoomId,
                    RoomNumber = dbRoom.RoomNumber,
                    DailyRate = dbRoom.DailyRate,
                    IsReserved = dbRoom.IsReserved
                });
                Console.WriteLine("✅ JSON: Room added.");
            }
            else
            {
                // update existing JSON room to match DB info
                fileRoom.RoomId = dbRoom.RoomId;
                fileRoom.DailyRate = dbRoom.DailyRate;
                fileRoom.IsReserved = dbRoom.IsReserved;
                _roomFileRepo.Update(fileRoom);
                Console.WriteLine("✅ JSON: Room updated.");
            }
        }

        // Show list of rooms from DB, then show list from JSON
        private void ViewAllRooms()
        {
            // read DB rooms
            Console.WriteLine("\n--- Database Rooms ---");
            var dbRooms = _hotelService.GetAllRooms().OrderBy(r => r.RoomNumber).ToList();
            if (dbRooms.Count == 0) Console.WriteLine("No rooms (DB).");
            foreach (var room in dbRooms)
                Console.WriteLine($"Room {room.RoomNumber} - Rate: {room.DailyRate:C} - {(room.IsReserved ? "Reserved" : "Available")}");

            // read JSON rooms
            Console.WriteLine("\n--- JSON Rooms ---");
            var fileRooms = _roomFileRepo.GetAll().OrderBy(r => r.RoomNumber).ToList();
            if (fileRooms.Count == 0) Console.WriteLine("No rooms (JSON).");
            foreach (var room in fileRooms)
                Console.WriteLine($"Room {room.RoomNumber} - Rate: {room.DailyRate:C} - {(room.IsReserved ? "Reserved" : "Available")}");
        }

        // Create a reservation in DB, then mirror it to JSON (reserve room, ensure guest, add booking)
        private void MakeReservation()
        {
            // read and validate guest name
            Console.Write("Enter Guest Name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > MaxNameLength)
            {
                Console.WriteLine("❌ Name is invalid.");
                return;
            }
            name = name.Trim();

            // read and validate room number
            Console.Write("Enter Room Number: ");
            if (!int.TryParse(Console.ReadLine(), out int roomNumber) || roomNumber <= 0)
            {
                Console.WriteLine("❌ Invalid room number.");
                return;
            }

            // read and validate nights
            Console.Write("Enter Number of Nights: ");
            if (!int.TryParse(Console.ReadLine(), out int nights) || nights <= 0)
            {
                Console.WriteLine("❌ Nights must be > 0.");
                return;
            }

            // fetch the DB room by number
            var roomDb = _hotelService.GetAllRooms().FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (roomDb == null)
            {
                Console.WriteLine("❌ DB: Room does not exist.");
                return;
            }

            // create reservation in DB via service
            var dbOk = _hotelService.TryReserveRoom(name, roomDb.RoomId, nights, out var dbMsg);
            Console.WriteLine(dbOk ? "✅ DB: " + dbMsg : "❌ DB: " + dbMsg);
            if (!dbOk) return; // stop if DB failed

            // ensure a JSON room exists for this number
            var rooms = _roomFileRepo.GetAll();
            var fileRoom = rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (fileRoom == null)
            {
                // insert JSON room aligned with DB properties
                fileRoom = new Room
                {
                    RoomId = roomDb.RoomId,
                    RoomNumber = roomDb.RoomNumber,
                    DailyRate = roomDb.DailyRate,
                    IsReserved = false
                };
                _roomFileRepo.Add(fileRoom);
            }

            // ensure a JSON guest exists (case-insensitive match)
            var guests = _guestFileRepo.GetAll();
            var fileGuest = guests.FirstOrDefault(g => g.Name != null && g.Name.Trim().ToLower() == name.ToLower());
            if (fileGuest == null)
            {
                fileGuest = new Guest
                {
                    GuestId = (guests.Count > 0 ? guests.Max(g => g.GuestId) + 1 : 1),
                    Name = name
                };
                _guestFileRepo.Add(fileGuest);
            }

            // mark room reserved in JSON
            if (!fileRoom.IsReserved)
            {
                fileRoom.IsReserved = true;
                _roomFileRepo.Update(fileRoom);
            }

            // add a JSON booking record
            var bookings = _bookingFileRepo.GetAll();
            var newBooking = new Booking
            {
                BookingId = bookings.Count > 0 ? bookings.Max(b => b.BookingId) + 1 : 1,
                RoomId = fileRoom.RoomId,
                GuestId = fileGuest.GuestId,
                Nights = nights,
                BookingDate = DateTime.Now
            };
            _bookingFileRepo.Add(newBooking);

            // confirm JSON mirroring
            Console.WriteLine("✅ JSON: Reservation mirrored.");
        }

        // Show bookings from DB, then show bookings from JSON
        private void ViewAllBookings()
        {
            // read DB bookings
            Console.WriteLine("\n--- Database Bookings ---");
            var dbBookings = _hotelService.GetAllBookings().OrderBy(b => b.BookingId).ToList();
            if (dbBookings.Count == 0) Console.WriteLine("No bookings (DB).");
            foreach (var b in dbBookings)
            {
                string guestName = b.Guest?.Name ?? "Unknown";
                int roomNo = b.Room?.RoomNumber ?? 0;
                double rate = b.Room?.DailyRate ?? 0;
                double total = rate * b.Nights;
                Console.WriteLine($"ID: {b.BookingId} | Date: {b.BookingDate:g} | Guest: {guestName} | Room #{roomNo} | Nights: {b.Nights} | Rate: {rate:C} | Total: {total:C}");
            }

            // read JSON bookings
            Console.WriteLine("\n--- JSON Bookings ---");
            var fileBookings = _bookingFileRepo.GetAll().OrderBy(b => b.BookingId).ToList();
            if (fileBookings.Count == 0) Console.WriteLine("No bookings (JSON).");

            // cross-load JSON rooms and guests to display details
            var rooms = _roomFileRepo.GetAll();
            var guests = _guestFileRepo.GetAll();

            foreach (var b in fileBookings)
            {
                var guest = guests.FirstOrDefault(g => g.GuestId == b.GuestId);
                var room = rooms.FirstOrDefault(r => r.RoomId == b.RoomId);
                string guestName = guest?.Name ?? "Unknown";
                int roomNo = room?.RoomNumber ?? 0;
                double rate = room?.DailyRate ?? 0;
                double total = rate * b.Nights;
                Console.WriteLine($"ID: {b.BookingId} | Date: {b.BookingDate:g} | Guest: {guestName} | Room #{roomNo} | Nights: {b.Nights} | Rate: {rate:C} | Total: {total:C}");
            }
        }

        // Cancel a booking in DB, then attempt to cancel the same ID in JSON
        private void CancelBooking()
        {
            // show current bookings for reference
            ViewAllBookings();
            Console.WriteLine();

            // read and validate booking id
            Console.Write("Enter Booking ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId) || bookingId <= 0)
            {
                Console.WriteLine("❌ Invalid ID.");
                return;
            }

            // cancel in DB via service
            var dbOk = _hotelService.TryCancelBooking(bookingId, out var dbMsg);
            Console.WriteLine(dbOk ? "✅ DB: " + dbMsg : "❌ DB: " + dbMsg);

            // cancel in JSON (IDs may not match DB, but we try same ID)
            var booking = _bookingFileRepo.GetById(bookingId);
            if (booking != null)
            {
                // unreserve the room in JSON if found
                var room = _roomFileRepo.GetById(booking.RoomId);
                if (room != null)
                {
                    room.IsReserved = false;
                    _roomFileRepo.Update(room);
                }

                // remove booking from JSON
                _bookingFileRepo.Delete(bookingId);
                Console.WriteLine("✅ JSON: Booking cancelled.");
            }
            else
            {
                Console.WriteLine("ℹ️ JSON: Booking ID not found.");
            }
        }

        // Search guest by name in DB and in JSON
        private void FindGuest()
        {
            // read and validate search text
            Console.Write("Enter guest name to search: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > MaxNameLength)
            {
                Console.WriteLine("❌ Name cannot be empty or too long.");
                return;
            }
            name = name.Trim();

            // DB search
            var dbGuest = _hotelService.FindGuestByName(name);
            if (dbGuest != null)
                Console.WriteLine($"✅ DB: Found Guest: {dbGuest.Name} (ID: {dbGuest.GuestId})");
            else
                Console.WriteLine("❌ DB: Guest not found.");

            // JSON search
            var fileGuest = _guestFileRepo.GetAll()
                .FirstOrDefault(g => g.Name != null && g.Name.ToLower().Contains(name.ToLower()));

            if (fileGuest != null)
                Console.WriteLine($"✅ JSON: Found Guest: {fileGuest.Name} (ID: {fileGuest.GuestId})");
            else
                Console.WriteLine("❌ JSON: Guest not found.");
        }

        // Show highest paying guest from DB, then show a best-effort from JSON
        private void ShowTopGuest()
        {
            // DB calculation
            Console.WriteLine("\n--- Database Highest-Paying Guest ---");
            var dbTop = _hotelService.GetHighestPayingGuest();
            if (dbTop != null)
                Console.WriteLine($"🏆 {dbTop.Name} (ID: {dbTop.GuestId})");
            else
                Console.WriteLine("No result (DB).");

            // JSON calculation
            Console.WriteLine("\n--- JSON Highest-Paying Guest ---");
            var bookings = _bookingFileRepo.GetAll();
            var rooms = _roomFileRepo.GetAll();
            var guests = _guestFileRepo.GetAll();

            // compute totals per guest from JSON data
            var top = bookings
                .GroupBy(b => b.GuestId)
                .Select(g => new
                {
                    GuestId = g.Key,
                    Total = g.Sum(b =>
                    {
                        var r = rooms.FirstOrDefault(x => x.RoomId == b.RoomId);
                        return r != null ? r.DailyRate * b.Nights : 0.0;
                    })
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefault();

            if (top != null && top.Total > 0)
            {
                var g = guests.FirstOrDefault(x => x.GuestId == top.GuestId);
                Console.WriteLine($"🏆 {g?.Name ?? "Unknown"} (Total: {top.Total:C})");
            }
            else
            {
                Console.WriteLine("No result (JSON).");
            }
        }

        // Add a review to DB, then add a review to JSON, both tied to the latest booking for the room
        private void AddReview()
        {
            // read and validate room id
            Console.Write("Enter Room ID: ");
            if (!int.TryParse(Console.ReadLine(), out int roomId) || roomId <= 0)
            {
                Console.WriteLine("❌ Invalid Room ID.");
                return;
            }

            // read and validate comment
            Console.Write("Enter your review comment: ");
            string comment = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(comment))
            {
                Console.WriteLine("❌ Comment cannot be empty.");
                return;
            }
            comment = comment.Trim();

            // read and validate rating
            Console.Write("Enter rating (1 to 5): ");
            if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
            {
                Console.WriteLine("❌ Invalid rating.");
                return;
            }

            // write to DB using DbContext with explicit dispose
            var context = new SilafHotelDbContext();
            try
            {
                // ensure room exists in DB
                var room = context.Rooms.Find(roomId);
                if (room == null)
                {
                    Console.WriteLine("❌ DB: Room not found.");
                }
                else
                {
                    // find the latest booking for this room in DB
                    var booking = context.Bookings
                        .Where(b => b.RoomId == roomId)
                        .OrderByDescending(b => b.BookingDate)
                        .FirstOrDefault();

                    if (booking == null)
                    {
                        Console.WriteLine("❌ DB: No bookings for this room.");
                    }
                    else
                    {
                        // add DB review record
                        context.Reviews.Add(new Review
                        {
                            GuestId = booking.GuestId,
                            RoomId = roomId,
                            Comment = comment,
                            Rating = rating
                        });
                        context.SaveChanges();
                        Console.WriteLine($"✅ DB: Review added for Room #{room.RoomNumber}, Guest ID {booking.GuestId}.");
                    }
                }
            }
            finally
            {
                // ensure DbContext is disposed
                context.Dispose();
            }

            // write to JSON
            var fileRoom = _roomFileRepo.GetById(roomId);
            if (fileRoom == null)
            {
                Console.WriteLine("❌ JSON: Room not found.");
                return;
            }

            var fileBooking = _bookingFileRepo.GetAll()
                .Where(b => b.RoomId == roomId)
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefault();

            if (fileBooking == null)
            {
                Console.WriteLine("❌ JSON: No bookings for this room.");
                return;
            }

            var reviews = FileContext.LoadReviews();
            var review = new Review
            {
                ReviewId = reviews.Count > 0 ? reviews.Max(r => r.ReviewId) + 1 : 1,
                GuestId = fileBooking.GuestId,
                RoomId = roomId,
                Comment = comment,
                Rating = rating
            };
            reviews.Add(review);
            FileContext.SaveReviews(reviews);
            Console.WriteLine($"✅ JSON: Review added for Room #{fileRoom.RoomNumber}, Guest ID {fileBooking.GuestId}.");
        }

        // Show all reviews from DB, then show all reviews from JSON
        private void ViewAllReviews()
        {
            // DB load with explicit dispose
            var context = new SilafHotelDbContext();
            try
            {
                var reviews = context.Reviews.ToList(); // list of DB reviews
                var guests = context.Guests.ToList();   // needed to display guest name
                var rooms = context.Rooms.ToList();     // needed to display room number

                Console.WriteLine("\n--- Database Reviews ---");
                if (reviews.Count == 0) Console.WriteLine("No reviews (DB).");

                foreach (var r in reviews)
                {
                    var guestName = guests.FirstOrDefault(g => g.GuestId == r.GuestId)?.Name ?? "Unknown";
                    var roomNum = rooms.FirstOrDefault(room => room.RoomId == r.RoomId)?.RoomNumber ?? 0;
                    Console.WriteLine($"Guest: {guestName}, Room #{roomNum}, Rating: {r.Rating}, Comment: {r.Comment}");
                }
            }
            finally
            {
                // dispose DbContext
                context.Dispose();
            }

            // JSON load
            Console.WriteLine("\n--- JSON Reviews ---");
            var fileReviews = FileContext.LoadReviews();
            var fileGuests = _guestFileRepo.GetAll();
            var fileRooms = _roomFileRepo.GetAll();

            if (fileReviews.Count == 0) Console.WriteLine("No reviews (JSON).");

            foreach (var r in fileReviews)
            {
                var guestName = fileGuests.FirstOrDefault(g => g.GuestId == r.GuestId)?.Name ?? "Unknown";
                var roomNum = fileRooms.FirstOrDefault(room => room.RoomId == r.RoomId)?.RoomNumber ?? 0;
                Console.WriteLine($"Guest: {guestName}, Room #{roomNum}, Rating: {r.Rating}, Comment: {r.Comment}");
            }
        }
    }
}
