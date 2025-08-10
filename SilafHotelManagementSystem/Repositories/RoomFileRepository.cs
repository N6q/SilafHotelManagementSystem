using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    /// <summary>
    /// Simple JSON-backed repository for <see cref="Room"/> records.
    /// Uses <see cref="FileContext"/> to load and save the entire collection each call.
    /// </summary>
    public class RoomFileRepository
    {
        /// <summary>
        /// Returns all rooms from the JSON file.
        /// </summary>
        public List<Room> GetAll()
        {
            // Load the full list of rooms from disk
            return FileContext.LoadRooms();
        }

        /// <summary>
        /// Returns a single room by its identifier, or null if not found.
        /// </summary>
        /// <param name="id">RoomId to search for.</param>
        public Room? GetById(int id)
        {
            // Load, then find the first room with a matching RoomId
            return FileContext.LoadRooms().FirstOrDefault(r => r.RoomId == id);
        }

        /// <summary>
        /// Appends a new room to the list and saves the file.
        /// </summary>
        /// <param name="room">The room to add (caller assigns RoomId).</param>
        public void Add(Room room)
        {
            // Load current snapshot
            var rooms = FileContext.LoadRooms();

            // Add the new room (no duplicate checks here)
            rooms.Add(room);

            // Persist updated list to disk
            FileContext.SaveRooms(rooms);
        }

        /// <summary>
        /// Replaces an existing room (matched by RoomId) and saves the file.
        /// If the room is not found, this is a no-op.
        /// </summary>
        /// <param name="updatedRoom">Room with same RoomId and new values.</param>
        public void Update(Room updatedRoom)
        {
            // Load current snapshot
            var rooms = FileContext.LoadRooms();

            // Find index of the target room
            var index = rooms.FindIndex(r => r.RoomId == updatedRoom.RoomId);

            // If present, replace and persist
            if (index >= 0)
            {
                rooms[index] = updatedRoom;
                FileContext.SaveRooms(rooms);
            }
            // else: silently ignore to keep API simple
        }

        /// <summary>
        /// Removes a room by id and saves the file.
        /// If the room is not found, nothing happens.
        /// </summary>
        /// <param name="id">RoomId to remove.</param>
        public void Delete(int id)
        {
            // Load current snapshot
            var rooms = FileContext.LoadRooms();

            // Find the room to remove
            var room = rooms.FirstOrDefault(r => r.RoomId == id);

            // If present, remove and persist
            if (room != null)
            {
                rooms.Remove(room);
                FileContext.SaveRooms(rooms);
            }
            // else: no-op
        }
    }
}
