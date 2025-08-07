using SilafHotelManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SilafHotelManagementSystem.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        public int RoomNumber { get; set; }

        [Range(100, double.MaxValue)]
        public double DailyRate { get; set; }

        public bool IsReserved { get; set; }

        // Navigation properties
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
