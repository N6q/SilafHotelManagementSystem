using Microsoft.EntityFrameworkCore;
using SilafHotelManagementSystem.Data;
using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories.Implementations
{
    /// <summary>
    /// EF Core repository for Review entities (synchronous).
    /// </summary>
    public class ReviewRepository : IReviewRepository
    {
        private readonly SilafHotelDbContext _context;

        public ReviewRepository(SilafHotelDbContext context)
        {
            _context = context;
        }

        /// <summary>Return all reviews with related Guest and Room (read-only).</summary>
        public List<Review> GetAll()
        {
            return _context.Reviews
                           .Include(r => r.Guest)
                           .Include(r => r.Room)
                           .AsNoTracking()
                           .ToList();
        }

        /// <summary>Find one review by id including Guest and Room; null if not found.</summary>
        public Review? GetById(int id)
        {
            return _context.Reviews
                           .Include(r => r.Guest)
                           .Include(r => r.Room)
                           .AsNoTracking()
                           .FirstOrDefault(r => r.ReviewId == id);
        }

        /// <summary>Insert a new review and save changes.</summary>
        public void Add(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        /// <summary>Update an existing review and save changes.</summary>
        public void Update(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        /// <summary>Delete a review by id (no-op if not found).</summary>
        public void Delete(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }

        /// <summary>Return all reviews for a given guest id (includes Room).</summary>
        public List<Review> GetByGuestId(int guestId)
        {
            return _context.Reviews
                           .Where(r => r.GuestId == guestId)
                           .Include(r => r.Room)
                           .AsNoTracking()
                           .ToList();
        }

        /// <summary>Return all reviews for a given room id (includes Guest).</summary>
        public List<Review> GetByRoomId(int roomId)
        {
            return _context.Reviews
                           .Where(r => r.RoomId == roomId)
                           .Include(r => r.Guest)
                           .AsNoTracking()
                           .ToList();
        }
    }
}























