
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;
using SilafHotelManagementSystem.Services.Interfaces;

namespace SilafHotelManagementSystem.Services
{
    public class HotelService : IHotelService
    {
        private readonly IRoomRepository _roomRepo;
        private readonly IGuestRepository _guestRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IReviewRepository _reviewRepo;

        public HotelService(
            IRoomRepository roomRepo,
            IGuestRepository guestRepo,
            IBookingRepository bookingRepo,
            IReviewRepository reviewRepo)
        {
            _roomRepo = roomRepo;
            _guestRepo = guestRepo;
            _bookingRepo = bookingRepo;
            _reviewRepo = reviewRepo;
        }
        public async Task AddRoomAsync(Room room)
        {
            await _roomRepo.AddAsync(room);
        }

        public async Task<List<Room>> GetAllRoomsAsync() => await _roomRepo.GetAllAsync();
        public async Task<List<Guest>> GetAllGuestsAsync() => await _guestRepo.GetAllAsync();
        public async Task<List<Booking>> GetAllBookingsAsync() => await _bookingRepo.GetAllAsync();
        public async Task<List<Review>> GetAllReviewsAsync() => await _reviewRepo.GetAllAsync();

        public async Task ReserveRoomAsync(string guestName, int roomId, int nights)
        {
            var room = await _roomRepo.GetByIdAsync(roomId);
            if (room == null || room.IsReserved)
                throw new Exception("Room is not available");

            var guest = (await _guestRepo.GetAllAsync()).FirstOrDefault(g => g.Name.ToLower() == guestName.ToLower());
            if (guest == null)
            {
                guest = new Guest { Name = guestName };
                await _guestRepo.AddAsync(guest);
            }

            room.IsReserved = true;
            await _roomRepo.UpdateAsync(room);

            var booking = new Booking
            {
                RoomId = room.RoomId,
                GuestId = guest.GuestId,
                Nights = nights,
                BookingDate = DateTime.Now
            };
            await _bookingRepo.AddAsync(booking);
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                throw new Exception("Booking not found");

            var room = await _roomRepo.GetByIdAsync(booking.RoomId);
            if (room != null)
            {
                room.IsReserved = false;
                await _roomRepo.UpdateAsync(room);
            }

            await _bookingRepo.DeleteAsync(bookingId);
        }

        public async Task<Guest?> FindGuestByNameAsync(string name)
        {
            var guests = await _guestRepo.GetAllAsync();
            return guests.FirstOrDefault(g => g.Name.ToLower().Contains(name.ToLower()));
        }

        public async Task<Guest?> GetHighestPayingGuestAsync()
        {
            var bookings = await _bookingRepo.GetAllAsync();

            var guestPayments = bookings
                .GroupBy(b => b.Guest)
                .Select(g => new
                {
                    Guest = g.Key,
                    Total = g.Sum(b => b.Nights * b.Room.DailyRate)
                })
                .OrderByDescending(g => g.Total)
                .FirstOrDefault();

            return guestPayments?.Guest;
        }
    }
}

