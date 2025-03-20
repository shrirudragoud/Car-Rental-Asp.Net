using System.ComponentModel.DataAnnotations;

namespace CarRentalWeb.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; } = "";

        [Required]
        public string Model { get; set; } = "";

        [Required]
        [Range(0.01, 10000)]
        [Display(Name = "Daily Rate")]
        public decimal DailyRate { get; set; }

        [Required]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = "";

        [Required]
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        // Navigation property
        public virtual ICollection<Booking>? Bookings { get; set; }

        public string GetFullName() => $"{Brand} {Model}";

        public string GetFormattedDailyRate() => $"${DailyRate:F2}";

        public IEnumerable<Booking> GetActiveBookings()
        {
            if (Bookings == null) return Enumerable.Empty<Booking>();
            
            return Bookings.Where(b => !b.IsCancelled && 
                b.EndDate >= DateTime.Today && 
                b.StartDate <= DateTime.Today.AddDays(30));
        }
    }
}