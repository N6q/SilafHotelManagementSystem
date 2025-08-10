using SilafHotelManagementSystem.Models;
using System.Collections.Generic;

namespace SilafHotelManagementSystem.Repositories.Interfaces
{
    /// <summary>
    /// Synchronous repository contract for Room entities.
    /// </summary>
    public interface IRoomRepository
    {
        List<Room> GetAll();     // read all rooms
        Room? GetById(int id);   // read one room by PK
        void Add(Room room);     // insert
        void Update(Room room);  // update
        void Delete(int id);     // delete (no-op if missing)
    }
}
