using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Models;
using BookShop.Models.UnitOfWork;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CitiesController : Controller
    {
        private readonly IUnitOfWork _UW;

        public CitiesController(IUnitOfWork UW)
        {
            _UW = UW;
        }

        // GET: Admin/Cities
        public async Task<IActionResult> Index(int id,int page = 1, int row = 10) // id==> ProvinceID
        {
            // Explicit Loading
            var Province = _UW._Context.Provinces.SingleAsync(p => p.ProvinceID == id);
            _UW._Context.Entry(await Province).Collection(c => c.City).Load();
            return View(Province.Result);
        }

        // GET: Admin/Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _UW._Context.Cities
                .Include(c => c.Province)
                .FirstOrDefaultAsync(m => m.CityID == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Admin/Cities/Create
        public IActionResult Create(int id)
        {
            City city = new City() { ProvinceID = id };
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CityID,CityName,ProvinceID")] City city)
        {
            if (ModelState.IsValid)
            {
                Random rdm = new Random();
                int RandomID = rdm.Next(400, 1000);
                var ExitID = await _UW.BaseRepository<City>().FindByIdAsync(RandomID);
                while (ExitID != null)
                {
                    RandomID = rdm.Next(400, 1000);
                    ExitID = await _UW.BaseRepository<City>().FindByIdAsync(RandomID);
                }

                City City = new City() { CityID = RandomID, CityName = city.CityName, ProvinceID = city.ProvinceID };
                await _UW.BaseRepository<City>().CreateAsync(City);
                await _UW.Commit();
                return RedirectToAction(nameof(Index), new { id = city.ProvinceID });
            }
            return View(city);
        }

        // GET: Admin/Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _UW.BaseRepository<City>().FindByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        // POST: Admin/Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CityID,CityName,ProvinceID")] City city)
        {
            if (id != city.CityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _UW.BaseRepository<City>().Update(city);
                    await _UW.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _UW.BaseRepository<Province>().FindByIdAsync(city.CityID) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = city.ProvinceID });
            }
            return View(city);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _UW.BaseRepository<City>().FindByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var City = await _UW.BaseRepository<City>().FindByIdAsync(id);
            if (City == null)
            {
                return NotFound();
            }

            else
            {
                _UW.BaseRepository<City>().Delete(City);
                await _UW.Commit();
                return RedirectToAction(nameof(Index), new { id = City.ProvinceID });
            }
        }
    }
}
