using Microsoft.EntityFrameworkCore;
using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilafHotelManagementSystem.Services
{
    public class HotelService : IHotelService
    {
        public bool TryAddRoom(Room room, out string message)
        {
            // simple input checks 
            if (room == null) { message = "Room is null."; return false; }
            if (room.RoomNumber <= 0) { message = "Room number must be > 0."; return false; }
            if (room.DailyRate <= 0) { message = "Daily rate must be > 0."; return false; }

            try
            {
                using (var context = new SilafHotelDbContext())
                {
                    if (context.Rooms.Any(r => r.RoomNumber == room.RoomNumber))
                    {
                        message = "Room number already exists.";
                        return false;
                    }

                    context.Rooms.Add(room);
                    context.SaveChanges();
                    message = "Room added.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Database error: " + ex.Message;
                return false;
            }
        }

        public List<Room> GetAllRooms()
        {
            try
            {
                using (var context = new SilafHotelDbContext())
                {
                    return context.Rooms.AsNoTracking().ToList();
                }
            }
            catch
            {
                return new List<Room>();
            }
        }

        public bool TryReserveRoom(string guestName, int roomId, int nights, out string message)
        {
            if (string.IsNullOrWhiteSpace(guestName)) { message = "Guest name required."; return false; }
            if (roomId <= 0) { message = "Invalid room id."; return false; }
            if (nights <= 0) { message = "Nights must be > 0."; return false; }

            try
            {
                using (var context = new SilafHotelDbContext())
                {
                    var room = context.Rooms.SingleOrDefault(r => r.RoomId == roomId);
                    if (room == null) { message = "Room not found."; return false; }
                    if (room.IsReserved) { message = "Room already reserved."; return false; }

                    string norm = guestName.Trim().ToLower();
                    var guest = context.Guests.FirstOrDefault(g => g.Name != null && g.Name.ToLower() == norm);
                    if (guest == null)
                    {
                        guest = new Guest { Name = guestName.Trim() };
                        context.Guests.Add(guest);
                        context.SaveChanges(); // ensure GuestId
                    }

                    context.Bookings.Add(new Booking
                    {
                        GuestId = guest.GuestId,
                        RoomId = room.RoomId,
                        Nights = nights,
                        BookingDate = DateTime.Now
                    });

                    room.IsReserved = true;
                    context.SaveChanges();

                    message = "Reservation completed.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Database error: " + ex.Message;
                return false;
            }
        }

        public List<Booking> GetAllBookings()
        {
            try
            {
                using (var context = new SilafHotelDbContext())
                {
                    return context.Bookings
                                  .Include(b => b.Guest)
                                  .Include(b => b.Room)
                                  .AsNoTracking()
                                  .ToList();
                }
            }
            catch
            {
                return new List<Booking>();
            }
        }

        public bool TryCancelBooking(int bookingId, out string message)
        {
            if (bookingId <= 0) { message = "Invalid booking id."; return false; }

            try
            {
                using (var context = new SilafHotelDbContext())
                {
                    var booking = context.Bookings.SingleOrDefault(b => b.BookingId == bookingId);
                    if (booking == null) { message = "Booking not found."; return false; }

                    var room = context.Rooms.SingleOrDefault(r => r.RoomId == booking.RoomId);
                    if (room != null) room.IsReserved = false;

                    context.Bookings.Remove(booking);
                    context.SaveChanges();

                    message = "Booking cancelled.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Database error: " + ex.Message;
                return false;
            }
        }

        public Guest? FindGuestByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            try
            {
                using (var context = new SilafHotelDbContext())
                {
                    string needle = name.Trim().ToLower();
                    return context.Guests
                                  .AsNoTracking()
                                  .FirstOrDefault(g => g.Name != null && g.Name.ToLower().Contains(needle));
                }
            }
            catch
            {
                return null;
            }
        }

        public Guest? GetHighestPayingGuest()
        {
            try
            {
                using (var context = new SilafHotelDbContext())
                {
                    var top = context.Bookings
                                     .Include(b => b.Room)
                                     .GroupBy(b => b.GuestId)
                                     .Select(g => new
                                     {
                                         GuestId = g.Key,
                                         Total = g.Sum(b => (b.Room != null ? b.Room.DailyRate : 0) * b.Nights)
                                     })
                                     .OrderByDescending(x => x.Total)
                                     .FirstOrDefault();

                    if (top == null || top.Total <= 0) return null;

                    return context.Guests.AsNoTracking().FirstOrDefault(g => g.GuestId == top.GuestId);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
