using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    public class RoomFileRepository
    {
        public List<Room> GetAll()
        {
            return FileContext.LoadRooms();
        }

        public Room? GetById(int id)
        {
            return FileContext.LoadRooms().FirstOrDefault(r => r.RoomId == id);
        }

        public void Add(Room room)
        {
            var rooms = FileContext.LoadRooms();
            rooms.Add(room);
            FileContext.SaveRooms(rooms);
        }

        public void Update(Room updatedRoom)
        {
            var rooms = FileContext.LoadRooms();
            var index = rooms.FindIndex(r => r.RoomId == updatedRoom.RoomId);
            if (index >= 0)
            {
                rooms[index] = updatedRoom;
                FileContext.SaveRooms(rooms);
            }
        }

        public void Delete(int id)
        {
            var rooms = FileContext.LoadRooms();
            var room = rooms.FirstOrDefault(r => r.RoomId == id);
            if (room != null)
            {
                rooms.Remove(room);
                FileContext.SaveRooms(rooms);
            }
        }
    }
}
