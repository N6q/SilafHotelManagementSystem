using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    public class GuestFileRepository
    {
        public List<Guest> GetAll() => FileContext.LoadGuests();

        public Guest? GetById(int id) =>
            FileContext.LoadGuests().FirstOrDefault(g => g.GuestId == id);

        public void Add(Guest guest)
        {
            var guests = FileContext.LoadGuests();
            guests.Add(guest);
            FileContext.SaveGuests(guests);
        }

        public void Update(Guest updatedGuest)
        {
            var guests = FileContext.LoadGuests();
            var index = guests.FindIndex(g => g.GuestId == updatedGuest.GuestId);
            if (index >= 0)
            {
                guests[index] = updatedGuest;
                FileContext.SaveGuests(guests);
            }
        }

        public void Delete(int id)
        {
            var guests = FileContext.LoadGuests();
            var guest = guests.FirstOrDefault(g => g.GuestId == id);
            if (guest != null)
            {
                guests.Remove(guest);
                FileContext.SaveGuests(guests);
            }
        }
    }
}
