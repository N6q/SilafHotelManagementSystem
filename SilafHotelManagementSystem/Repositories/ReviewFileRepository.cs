using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    /// <summary>
    /// Simple JSON-backed repository for <see cref="Review"/> records.
    /// Uses <see cref="FileContext"/> to load and save the entire collection each call.
    /// </summary>
    public class ReviewFileRepository
    {
        /// <summary>
        /// Returns all reviews from the JSON file.
        /// </summary>
        public List<Review> GetAll() => FileContext.LoadReviews();

        /// <summary>
        /// Returns a single review by its identifier, or null if not found.
        /// </summary>
        /// <param name="id">ReviewId to search for.</param>
        public Review? GetById(int id) =>
            FileContext.LoadReviews().FirstOrDefault(r => r.ReviewId == id);

        /// <summary>
        /// Appends a new review to the list and saves the file.
        /// </summary>
        /// <param name="review">The review to add (caller assigns ReviewId).</param>
        public void Add(Review review)
        {
            // Load current snapshot from disk
            var reviews = FileContext.LoadReviews();

            // Add the new item (no duplicate check here)
            reviews.Add(review);

            // Persist updated list to disk
            FileContext.SaveReviews(reviews);
        }

        /// <summary>
        /// Replaces an existing review (matched by ReviewId) and saves the file.
        /// If the review is not found, this is a no-op.
        /// </summary>
        /// <param name="updatedReview">Review with same ReviewId and new values.</param>
        public void Update(Review updatedReview)
        {
            // Load current snapshot
            var reviews = FileContext.LoadReviews();

            // Find the index for the matching ReviewId
            var index = reviews.FindIndex(r => r.ReviewId == updatedReview.ReviewId);

            // If found, replace and persist
            if (index >= 0)
            {
                reviews[index] = updatedReview;
                FileContext.SaveReviews(reviews);
            }
            // else: silently ignore to keep API simple
        }

        /// <summary>
        /// Removes a review by id and saves the file.
        /// If the review is not found, nothing happens.
        /// </summary>
        /// <param name="id">ReviewId to remove.</param>
        public void Delete(int id)
        {
            // Load current snapshot
            var reviews = FileContext.LoadReviews();

            // Find the target review
            var review = reviews.FirstOrDefault(r => r.ReviewId == id);

            // If present, remove and persist
            if (review != null)
            {
                reviews.Remove(review);
                FileContext.SaveReviews(reviews);
            }
            // else: no-op
        }
    }
}
