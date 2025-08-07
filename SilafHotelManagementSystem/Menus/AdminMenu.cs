using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories;
using SilafHotelManagementSystem.Repositories.Interfaces;
using SilafHotelManagementSystem.Services.Interfaces;
using System;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SilafHotelManagementSystem.Menus
{
    public class AdminMenu
    {
        private readonly IHotelService _hotelService;
        private readonly RoomFileRepository _roomFileRepo = new RoomFileRepository();
        private readonly GuestFileRepository _guestFileRepo = new GuestFileRepository();
        private readonly BookingFileRepository _bookingFileRepo = new BookingFileRepository();

        private bool _useFileMode;

        public AdminMenu(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public async Task ShowAsync()
        {
            Console.Clear();
            Console.WriteLine("Choose Storage Mode:");
            Console.WriteLine("1. Database (EF Core)");
            Console.WriteLine("2. File System (JSON)");
            Console.Write("Enter choice: ");
            string mode = Console.ReadLine();
            _useFileMode = mode == "2";

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== Silaf Hotel Management System ====");
                Console.WriteLine("1. Add New Room");
                Console.WriteLine("2. View All Rooms");
                Console.WriteLine("3. Make a Reservation");
                Console.WriteLine("4. Cancel a Booking");
                Console.WriteLine("5. Find Guest by Name");
                Console.WriteLine("6. Show Highest-Paying Guest");
                Console.WriteLine("7. Add Review");
                Console.WriteLine("8. View All Reviews");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                           await AddRoom();
                            break;

                        case "2":
                            await ViewAllRooms();
                            break;
                        case "3":
                            await MakeReservation();
                            break;
                        case "4":
                            await CancelBooking();
                            break;
                        case "5":
                            await FindGuest();
                            break;
                        case "6":
                            await ShowTopGuest();
                            break;
                        case "7":
                            await AddReview();
                            break;

                        case "8":
                            await ViewAllReviews();
                            break;


                        case "0":
                            Console.WriteLine("Exiting...");
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }

                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }

        private async Task AddRoom()
        {
            Console.Write("\nEnter Room Number: ");
            int number = int.Parse(Console.ReadLine());

            Console.Write("Enter Daily Rate: ");
            double rate = double.Parse(Console.ReadLine());

            var room = new Room
            {
                RoomNumber = number,
                DailyRate = rate,
                IsReserved = false
            };

            if (_useFileMode)
            {
                var rooms = _roomFileRepo.GetAll();
                room.RoomId = rooms.Count > 0 ? rooms.Max(r => r.RoomId) + 1 : 1;
                _roomFileRepo.Add(room);
                Console.WriteLine("✅ Room added (file mode).");
            }
            else
            {
                await _hotelService.AddRoomAsync(room);
                Console.WriteLine("✅ Room added (database).");
            }
        }

        private async Task ViewAllRooms()
        {
            Console.WriteLine("--- Room List ---");

            if (_useFileMode)
            {
                var rooms = _roomFileRepo.GetAll();
                foreach (var room in rooms)
                    Console.WriteLine($"Room {room.RoomNumber} - Rate: {room.DailyRate:C} - {(room.IsReserved ? "Reserved" : "Available")}");
            }
            else
            {
                var rooms = await _hotelService.GetAllRoomsAsync();
                foreach (var room in rooms)
                    Console.WriteLine($"Room {room.RoomNumber} - Rate: {room.DailyRate:C} - {(room.IsReserved ? "Reserved" : "Available")}");
            }
        }

        private async Task MakeReservation()
        {
            Console.Write("Enter Guest Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Room Number: ");
            int roomNumber = int.Parse(Console.ReadLine());

            Console.Write("Enter Number of Nights: ");
            int nights = int.Parse(Console.ReadLine());

            if (_useFileMode)
            {
                var room = _roomFileRepo.GetAll().FirstOrDefault(r => r.RoomNumber == roomNumber);
                if (room == null || room.IsReserved)
                {
                    Console.WriteLine("❌ Room is not available.");
                    return;
                }

                var guests = _guestFileRepo.GetAll();
                var guest = guests.FirstOrDefault(g => g.Name.ToLower() == name.ToLower());
                if (guest == null)
                {
                    guest = new Guest { GuestId = guests.Count + 1, Name = name };
                    _guestFileRepo.Add(guest);
                }

                room.IsReserved = true;
                _roomFileRepo.Update(room);

                var bookings = _bookingFileRepo.GetAll();
                var booking = new Booking
                {
                    BookingId = bookings.Count + 1,
                    RoomId = room.RoomId,
                    GuestId = guest.GuestId,
                    Nights = nights,
                    BookingDate = DateTime.Now
                };
                _bookingFileRepo.Add(booking);

                Console.WriteLine("✅ Reservation successful (file mode).");
            }
            else
            {
                var rooms = await _hotelService.GetAllRoomsAsync();
                var room = rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
                if (room == null || room.IsReserved)
                {
                    Console.WriteLine("❌ Room is not available.");
                    return;
                }

                await _hotelService.ReserveRoomAsync(name, room.RoomId, nights);
                Console.WriteLine("✅ Reservation successful (database).");
            }
        }

        private async Task CancelBooking()
        {
            Console.Write("Enter Booking ID to cancel: ");
            int bookingId = int.Parse(Console.ReadLine());

            if (_useFileMode)
            {
                var booking = _bookingFileRepo.GetById(bookingId);
                if (booking == null)
                {
                    Console.WriteLine("❌ Booking not found.");
                    return;
                }

                var room = _roomFileRepo.GetById(booking.RoomId);
                if (room != null)
                {
                    room.IsReserved = false;
                    _roomFileRepo.Update(room);
                }

                _bookingFileRepo.Delete(bookingId);
                Console.WriteLine("✅ Booking cancelled (file mode).");
            }
            else
            {
                await _hotelService.CancelBookingAsync(bookingId);
                Console.WriteLine("✅ Booking cancelled (database).");
            }
        }

        private async Task FindGuest()
        {
            Console.Write("Enter guest name to search: ");
            string name = Console.ReadLine();

            if (_useFileMode)
            {
                var guest = _guestFileRepo.GetAll().FirstOrDefault(g => g.Name.ToLower().Contains(name.ToLower()));
                if (guest != null)
                    Console.WriteLine($"✅ Found Guest: {guest.Name} (ID: {guest.GuestId})");
                else
                    Console.WriteLine("❌ Guest not found.");
            }
            else
            {
                var guest = await _hotelService.FindGuestByNameAsync(name);
                if (guest != null)
                    Console.WriteLine($"✅ Found Guest: {guest.Name} (ID: {guest.GuestId})");
                else
                    Console.WriteLine("❌ Guest not found.");
            }
        }

        private async Task ShowTopGuest()
        {
            if (_useFileMode)
            {
                var bookings = _bookingFileRepo.GetAll();
                var rooms = _roomFileRepo.GetAll();
                var guests = _guestFileRepo.GetAll();

                var guestTotals = bookings
                    .GroupBy(b => b.GuestId)
                    .Select(g => new
                    {
                        GuestId = g.Key,
                        Total = g.Sum(b => rooms.FirstOrDefault(r => r.RoomId == b.RoomId)?.DailyRate * b.Nights ?? 0)
                    })
                    .OrderByDescending(x => x.Total)
                    .FirstOrDefault();

                if (guestTotals != null)
                {
                    var topGuest = guests.FirstOrDefault(g => g.GuestId == guestTotals.GuestId);
                    Console.WriteLine($"🏆 Highest Paying Guest: {topGuest?.Name} (Total: {guestTotals.Total:C})");
                }
                else
                {
                    Console.WriteLine("❌ No bookings found.");
                }
            }
            else
            {
                var guest = await _hotelService.GetHighestPayingGuestAsync();
                if (guest != null)
                    Console.WriteLine($"🏆 Highest Paying Guest: {guest.Name} (ID: {guest.GuestId})");
                else
                    Console.WriteLine("❌ No guests found.");
            }
        }
        private async Task AddReview()
        {
            Console.Write("Enter Guest ID: ");
            int guestId = int.Parse(Console.ReadLine());

            Console.Write("Enter Room ID: ");
            int roomId = int.Parse(Console.ReadLine());

            Console.Write("Enter your review comment: ");
            string comment = Console.ReadLine();

            Console.Write("Enter rating (1 to 5): ");
            int rating = int.Parse(Console.ReadLine());

            var review = new Review
            {
                GuestId = guestId,
                RoomId = roomId,
                Comment = comment,
                Rating = rating
            };

            if (_useFileMode)
            {
                var reviews = FileContext.LoadReviews();
                review.ReviewId = reviews.Count > 0 ? reviews.Max(r => r.ReviewId) + 1 : 1;
                reviews.Add(review);
                FileContext.SaveReviews(reviews);
                Console.WriteLine("✅ Review added (file mode).");
            }
            else
            {
                using var context = new SilafHotelDbContext();
                context.Reviews.Add(review);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Review added (database).");
            }
        }
        private async Task ViewAllReviews()
        {
            Console.WriteLine("--- All Reviews ---");

            if (_useFileMode)
            {
                var reviews = FileContext.LoadReviews();
                var guests = _guestFileRepo.GetAll();
                var rooms = _roomFileRepo.GetAll();

                foreach (var r in reviews)
                {
                    var guestName = guests.FirstOrDefault(g => g.GuestId == r.GuestId)?.Name ?? "Unknown";
                    var roomNum = rooms.FirstOrDefault(room => room.RoomId == r.RoomId)?.RoomNumber ?? 0;
                    Console.WriteLine($"Guest: {guestName}, Room #{roomNum}, Rating: {r.Rating}, Comment: {r.Comment}");
                }
            }
            else
            {
                using var context = new SilafHotelDbContext();
                var reviews = context.Reviews.ToList();
                var guests = context.Guests.ToList();
                var rooms = context.Rooms.ToList();

                foreach (var r in reviews)
                {
                    var guestName = guests.FirstOrDefault(g => g.GuestId == r.GuestId)?.Name ?? "Unknown";
                    var roomNum = rooms.FirstOrDefault(room => room.RoomId == r.RoomId)?.RoomNumber ?? 0;
                    Console.WriteLine($"Guest: {guestName}, Room #{roomNum}, Rating: {r.Rating}, Comment: {r.Comment}");
                }
            }
        }


    }
}
