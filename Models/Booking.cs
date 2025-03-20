using System.ComponentModel.DataAnnotations;

namespace CarRentalWeb.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        [Range(0.01, 100000)]
        public decimal TotalPrice { get; set; }

        public bool IsCancelled { get; set; }

        // Navigation property
        public virtual Car? Car { get; set; }

        public bool IsActive() => 
            !IsCancelled && StartDate <= DateTime.Today && EndDate >= DateTime.Today;

        public bool IsUpcoming() => 
            !IsCancelled && StartDate > DateTime.Today;

        public bool IsPast() => 
            EndDate < DateTime.Today || IsCancelled;

        public bool IsValidDates()
        {
            if (StartDate > EndDate) return false;
            if (StartDate < DateTime.Today) return false;
            return true;
        }

        public string GetStatusBadgeClass()
        {
            if (IsCancelled) return "bg-danger";
            if (IsActive()) return "bg-success";
            if (IsUpcoming()) return "bg-info";
            return "bg-secondary";
        }

        public string GetStatusText()
        {
            if (IsCancelled) return "Cancelled";
            if (IsActive()) return "Active";
            if (IsUpcoming()) return "Upcoming";
            return "Completed";
        }

        public string GetFormattedPrice() => $"${TotalPrice:F2}";

        public string GetFormattedDates() => $"{StartDate:MMM dd, yyyy} - {EndDate:MMM dd, yyyy}";

        public int GetDurationInDays() => (EndDate - StartDate).Days + 1;
    }
}