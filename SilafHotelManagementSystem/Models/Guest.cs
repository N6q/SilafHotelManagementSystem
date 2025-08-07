using SilafHotelManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SilafHotelManagementSystem.Models
{
    public class Guest
    {
        [Key]
        public int GuestId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation properties
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
