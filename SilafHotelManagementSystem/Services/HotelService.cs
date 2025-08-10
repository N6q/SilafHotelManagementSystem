using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;
using SilafHotelManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilafHotelManagementSystem.Services
{
    /// <summary>
    /// Synchronous domain service. All DB access happens via repositories,
    /// which share the SAME DbContext instance created in Program.cs.
    /// </summary>
    public class HotelService : IHotelService
    {
        // Repositories (all backed by the same DbContext instance from Program.cs)
        private readonly IRoomRepository _rooms;
        private readonly IGuestRepository _guests;
        private readonly IBookingRepository _bookings;
        private readonly IReviewRepository _reviews;

        /// <summary>
        /// Construct the service with repositories 
        /// </summary>
        public HotelService(
            IRoomRepository rooms,
            IGuestRepository guests,
            IBookingRepository bookings,
            IReviewRepository reviews)
        {
            _rooms = rooms;
            _guests = guests;
            _bookings = bookings;
            _reviews = reviews;
        }

        /// <summary>
        /// Add a room to the database (simple validation + unique RoomNumber).
        /// </summary>
        public bool TryAddRoom(Room room, out string message)
        {
            if (room == null) { message = "Room is null."; return false; }
            if (room.RoomNumber <= 0) { message = "Room number must be > 0."; return false; }
            if (room.DailyRate <= 0) { message = "Daily rate must be > 0."; return false; }

            // Prevent duplicate room numbers
            if (_rooms.GetAll().Any(r => r.RoomNumber == room.RoomNumber))
            {
                message = "Room number already exists.";
                return false;
            }

            _rooms.Add(room);             // repo saves via shared DbContext
            message = "Room added.";
            return true;
        }

        /// <summary>Return all rooms.</summary>
        public List<Room> GetAllRooms() => _rooms.GetAll();

        /// <summary>
        /// Reserve a room: ensure guest exists, add booking, mark room reserved.
        /// </summary>
        public bool TryReserveRoom(string guestName, int roomId, int nights, out string message)
        {
            if (string.IsNullOrWhiteSpace(guestName)) { message = "Guest name required."; return false; }
            if (roomId <= 0) { message = "Invalid room id."; return false; }
            if (nights <= 0) { message = "Nights must be > 0."; return false; }

            var room = _rooms.GetById(roomId);
            if (room == null) { message = "Room not found."; return false; }
            if (room.IsReserved) { message = "Room already reserved."; return false; }

            // find or create guest (case-insensitive)
            var norm = guestName.Trim().ToLower();
            var guest = _guests.GetAll().FirstOrDefault(g => g.Name != null && g.Name.ToLower() == norm);
            if (guest == null)
            {
                guest = new Guest { Name = guestName.Trim() };
                _guests.Add(guest);
            }

            // add booking
            _bookings.Add(new Booking
            {
                GuestId = guest.GuestId,
                RoomId = room.RoomId,
                Nights = nights,
                BookingDate = DateTime.Now
            });

            // update room state
            room.IsReserved = true;
            _rooms.Update(room);

            message = "Reservation completed.";
            return true;
        }

        /// <summary>Return all bookings (repo includes Guest and Room).</summary>
        public List<Booking> GetAllBookings() => _bookings.GetAll();

        /// <summary>
        /// Cancel a booking and free the room if present.
        /// </summary>
        public bool TryCancelBooking(int bookingId, out string message)
        {
            if (bookingId <= 0) { message = "Invalid booking id."; return false; }

            var booking = _bookings.GetById(bookingId);
            if (booking == null) { message = "Booking not found."; return false; }

            var room = _rooms.GetById(booking.RoomId);
            if (room != null)
            {
                room.IsReserved = false;
                _rooms.Update(room);
            }

            _bookings.Delete(bookingId);
            message = "Booking cancelled.";
            return true;
        }

        /// <summary>Find guest by partial name (case-insensitive).</summary>
        public Guest? FindGuestByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var needle = name.Trim().ToLower();
            return _guests.GetAll()
                          .FirstOrDefault(g => g.Name != null && g.Name.ToLower().Contains(needle));
        }

        /// <summary>
        /// Compute the highest-paying guest from bookings (sum rate * nights).
        /// </summary>
        public Guest? GetHighestPayingGuest()
        {
            var bookings = _bookings.GetAll(); // includes Room and Guest (via repo)
            if (bookings.Count == 0) return null;

            var top = bookings
                .GroupBy(b => b.GuestId)
                .Select(g => new
                {
                    GuestId = g.Key,
                    Total = g.Sum(b => (b.Room != null ? b.Room.DailyRate : 0) * b.Nights)
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefault();

            if (top == null || top.Total <= 0) return null;

            return _guests.GetById(top.GuestId);
        }
    }
}
