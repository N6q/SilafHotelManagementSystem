using Microsoft.EntityFrameworkCore;
using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories.Implementations
{
    /// <summary>
    /// EF Core repository for Room entities (synchronous).
    /// </summary>
    public class RoomRepository : IRoomRepository
    {
        private readonly SilafHotelDbContext _context;

        public RoomRepository(SilafHotelDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return all rooms (read-only, no tracking).
        /// </summary>
        public List<Room> GetAll()
        {
            return _context.Rooms
                           .AsNoTracking()
                           .ToList();          // <-- sync ToList (no await)
        }

        /// <summary>
        /// Find a room by primary key; null if not found.
        /// </summary>
        public Room? GetById(int id)
        {
            return _context.Rooms.Find(id);       // <-- sync Find
        }

        /// <summary>
        /// Insert a new room and save.
        /// </summary>
        public void Add(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();               // <-- sync SaveChanges
        }

        /// <summary>
        /// Update existing room and save.
        /// </summary>
        public void Update(Room room)
        {
            _context.Rooms.Update(room);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete by id (no-op if not found).
        /// </summary>
        public void Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
        }
    }
}
