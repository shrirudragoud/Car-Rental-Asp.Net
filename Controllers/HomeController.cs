    using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalWeb.Data;
using CarRentalWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace CarRentalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get available cars
            var availableCars = await _context.Cars
                .Where(c => c.IsAvailable)
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Take(6)
                .ToListAsync();

            // Get user's active bookings if logged in
            var userBookings = new List<Booking>();
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                userBookings = await _context.Bookings
                    .Include(b => b.Car)
                    .Where(b => b.UserId == currentUser.Id && b.EndDate >= DateTime.Today)
                    .OrderBy(b => b.StartDate)
                    .Take(3)
                    .ToListAsync();
            }

            // Get statistics for admin
            var stats = new Dictionary<string, int>();
            if (User.IsInRole("Admin"))
            {
                stats.Add("TotalCars", await _context.Cars.CountAsync());
                stats.Add("AvailableCars", await _context.Cars.CountAsync(c => c.IsAvailable));
                stats.Add("ActiveBookings", await _context.Bookings.CountAsync(b => 
                    b.StartDate <= DateTime.Today && b.EndDate >= DateTime.Today));
                stats.Add("TotalUsers", await _userManager.Users.CountAsync());
            }

            ViewBag.UserBookings = userBookings;
            ViewBag.Stats = stats;

            return View(availableCars);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Home/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found.";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Sorry, you don't have permission to access this resource.";
                    break;
                default:
                    ViewBag.ErrorMessage = "Sorry, something went wrong on the server.";
                    break;
            }

            return View("Error");
        }
    }
}
