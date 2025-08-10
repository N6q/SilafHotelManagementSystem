using Microsoft.EntityFrameworkCore;
using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories.Implementations
{
    /// <summary>
    /// EF Core repository for Guest entities (synchronous).
    /// </summary>
    public class GuestRepository : IGuestRepository
    {
        private readonly SilafHotelDbContext _context;

        public GuestRepository(SilafHotelDbContext context)
        {
            _context = context;
        }

        /// <summary>Return all guests (read-only).</summary>
        public List<Guest> GetAll()
        {
            return _context.Guests
                           .AsNoTracking()
                           .ToList();
        }

        /// <summary>Find a guest by primary key; null if not found.</summary>
        public Guest? GetById(int id)
        {
            return _context.Guests.Find(id);
        }

        /// <summary>Insert a new guest and save changes.</summary>
        public void Add(Guest guest)
        {
            _context.Guests.Add(guest);
            _context.SaveChanges();
        }

        /// <summary>Update an existing guest and save changes.</summary>
        public void Update(Guest guest)
        {
            _context.Guests.Update(guest);
            _context.SaveChanges();
        }

        /// <summary>Delete a guest by id (no-op if not found).</summary>
        public void Delete(int id)
        {
            var guest = _context.Guests.Find(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                _context.SaveChanges();
            }
        }
    }
}
