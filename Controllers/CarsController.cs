using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalWeb.Data;
using CarRentalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace CarRentalWeb.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.ToListAsync();
            return View(cars);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSampleCars()
        {
            var sampleCars = new List<Car>
            {
                new Car
                {
                    Brand = "Toyota",
                    Model = "Camry",
                    DailyRate = 89.99M,
                    ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Toyota/Camry/10921/1690544152615/front-left-side-47.jpg",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Honda",
                    Model = "Civic",
                    DailyRate = 79.99M,
                    ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Honda/Civic/7740/1584360708736/front-left-side-47.jpg",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "BMW",
                    Model = "3 Series",
                    DailyRate = 129.99M,
                    ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/BMW/3-Series/10574/1689589130794/front-left-side-47.jpg",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Mercedes-Benz",
                    Model = "C-Class",
                    DailyRate = 139.99M,
                    ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Mercedes-Benz/C-Class/10858/1690027866883/front-left-side-47.jpg",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Audi",
                    Model = "A4",
                    DailyRate = 119.99M,
                    ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Audi/A4/10369/1689675202960/front-left-side-47.jpg",
                    IsAvailable = true
                }
            };

            foreach (var car in sampleCars)
            {
                // Check if image URL is accessible
                try
                {
                    var request = WebRequest.Create(car.ImageUrl);
                    request.Method = "HEAD";
                    using (var response = await request.GetResponseAsync())
                    {
                        if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                        {
                            car.ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Toyota/Camry/10921/1690544152615/front-left-side-47.jpg";
                        }
                    }
                }
                catch
                {
                    car.ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Toyota/Camry/10921/1690544152615/front-left-side-47.jpg";
                }

                _context.Cars.Add(car);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Sample cars added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,DailyRate,ImageUrl,IsAvailable")] Car car)
        {
            if (ModelState.IsValid)
            {
                // Validate image URL
                if (!string.IsNullOrEmpty(car.ImageUrl))
                {
                    try
                    {
                        var request = WebRequest.Create(car.ImageUrl);
                        request.Method = "HEAD";
                        using (var response = await request.GetResponseAsync())
                        {
                            if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                            {
                                car.ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Toyota/Camry/10921/1690544152615/front-left-side-47.jpg";
                            }
                        }
                    }
                    catch
                    {
                        car.ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Toyota/Camry/10921/1690544152615/front-left-side-47.jpg";
                    }
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,DailyRate,ImageUrl,IsAvailable")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validate image URL
                    if (!string.IsNullOrEmpty(car.ImageUrl))
                    {
                        try
                        {
                            var request = WebRequest.Create(car.ImageUrl);
                            request.Method = "HEAD";
                            using (var response = await request.GetResponseAsync())
                            {
                                if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                                {
                                    car.ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Toyota/Camry/10921/1690544152615/front-left-side-47.jpg";
                                }
                            }
                        }
                        catch
                        {
                            car.ImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Toyota/Camry/10921/1690544152615/front-left-side-47.jpg";
                        }
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}