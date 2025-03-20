using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarRentalWeb.Data;
using CarRentalWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalWeb.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == currentUser.Id)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUser.Id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        public async Task<IActionResult> Create(int carId)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == carId && c.IsAvailable);

            if (car == null)
            {
                TempData["Error"] = "Car not found or not available.";
                return RedirectToAction("Index", "Cars");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var booking = new Booking
            {
                CarId = car.Id,
                Car = car,
                UserId = currentUser.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarId,StartDate,EndDate")] Booking booking)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == booking.CarId && c.IsAvailable);

            if (car == null)
            {
                ModelState.AddModelError("", "Car not found or not available.");
                return View(booking);
            }

            booking.UserId = currentUser.Id;
            booking.Car = car;

            if (!booking.IsValidDates())
            {
                ModelState.AddModelError("", "Invalid dates selected. Start date must be today or later, and end date must be after start date.");
                return View(booking);
            }

            var hasOverlap = await _context.Bookings
                .Where(b => b.CarId == booking.CarId && !b.IsCancelled)
                .Where(b => b.StartDate <= booking.EndDate && b.EndDate >= booking.StartDate)
                .AnyAsync();

            if (hasOverlap)
            {
                ModelState.AddModelError("", "This car is already booked for the selected dates.");
                return View(booking);
            }

            var days = (booking.EndDate - booking.StartDate).Days + 1;
            booking.TotalPrice = car.DailyRate * days;

            _context.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking confirmed successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUser.Id);

            if (booking == null)
            {
                return NotFound();
            }

            if (!booking.IsUpcoming())
            {
                TempData["Error"] = "Only upcoming bookings can be cancelled.";
                return RedirectToAction(nameof(Details), new { id = booking.Id });
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUser.Id);

            if (booking == null)
            {
                return NotFound();
            }

            if (!booking.IsUpcoming())
            {
                TempData["Error"] = "Only upcoming bookings can be cancelled.";
                return RedirectToAction(nameof(Details), new { id = booking.Id });
            }

            booking.IsCancelled = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}