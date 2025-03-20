using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalWeb.Data;
using CarRentalWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalWeb.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Cars.ToListAsync());
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
                    ImageUrl = "https://imgd.aeplcdn.com/664x374/n/cw/ec/192443/camry-exterior-right-front-three-quarter-2.jpeg?isig=0&q=80",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Honda",
                    Model = "Civic",
                    DailyRate = 79.99M,
                    ImageUrl = "https://imgd.aeplcdn.com/664x374/n/cw/ec/27074/civic-exterior-right-front-three-quarter-148156.jpeg?q=80",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Ford",
                    Model = "Mustang GT",
                    DailyRate = 199.99M,
                    ImageUrl = "https://www.vdm.ford.com/content/dam/na/ford/en_us/images/mustang/2025/jellybeans/Ford_Mustang_2025_101A_PYZ_882_89W_13A_CON_64F_99H_44U_EBST_DEFAULT_EXT_4.png",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "Tesla",
                    Model = "Model 3",
                    DailyRate = 149.99M,
                    ImageUrl = "https://cdn.prod.website-files.com/60ce1b7dd21cd517bb39ff20/6153cdf8aec0a73b65af24c0_tesla-model-3-p-1080.png",
                    IsAvailable = true
                },
                new Car
                {
                    Brand = "BMW",
                    Model = "3 Series",
                    DailyRate = 129.99M,
                    ImageUrl = "https://www.bmw.in/content/dam/bmw/marketIN/bmw_in/all-models/3-series/gl/2023/navigation.png/jcr:content/renditions/cq5dam.resized.img.585.low.time1673330685738.png",
                    IsAvailable = true
                }
            };

            await _context.Cars.AddRangeAsync(sampleCars);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sample cars added successfully!";
            return RedirectToAction(nameof(Index));
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Brand,Model,DailyRate,ImageUrl,IsAvailable")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Car added successfully!";
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
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Car updated successfully!";
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
                TempData["Success"] = "Car deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}