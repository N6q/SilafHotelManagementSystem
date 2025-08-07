using SilafHotelManagementSystem.Models;
using SilafHotelManagementSystem.Data;
using System.Linq;

namespace SilafHotelManagementSystem.Repositories
{
    public class ReviewFileRepository
    {
        public List<Review> GetAll() => FileContext.LoadReviews();

        public Review? GetById(int id) =>
            FileContext.LoadReviews().FirstOrDefault(r => r.ReviewId == id);

        public void Add(Review review)
        {
            var reviews = FileContext.LoadReviews();
            reviews.Add(review);
            FileContext.SaveReviews(reviews);
        }

        public void Update(Review updatedReview)
        {
            var reviews = FileContext.LoadReviews();
            var index = reviews.FindIndex(r => r.ReviewId == updatedReview.ReviewId);
            if (index >= 0)
            {
                reviews[index] = updatedReview;
                FileContext.SaveReviews(reviews);
            }
        }

        public void Delete(int id)
        {
            var reviews = FileContext.LoadReviews();
            var review = reviews.FirstOrDefault(r => r.ReviewId == id);
            if (review != null)
            {
                reviews.Remove(review);
                FileContext.SaveReviews(reviews);
            }
        }
    }
}
