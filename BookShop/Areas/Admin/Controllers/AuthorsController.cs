using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BookShop.Models.UnitOfWork;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorsController : Controller
    {
        private readonly IUnitOfWork _UW;

        public AuthorsController(IUnitOfWork UW)
        {
            _UW = UW;
        }

        // GET: Admin/Authors
        public async Task<IActionResult> Index(int page = 1, int row = 10, string sortExpression = "FirstName", string firstName = "")
        {
            //var Authors = await _context.Authors.ToListAsync();
            var Authors = _UW.BaseRepository<Author>().FindAllAsync();
            var PagingModel = PagingList.Create(await Authors, row, page,sortExpression,"FirstName");
            //ViewBag.EntityStates = DisplayStates(_context.ChangeTracker.Entries());
            PagingModel.RouteValue = new RouteValueDictionary
            {
                {"row",row},
                {"FirstName",firstName}
            };
            return View(PagingModel);
        }

        // GET: Admin/Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var author = await _context.Authors
            //    .FirstOrDefaultAsync(m => m.AuthorID == id);
            var author = await _UW.BaseRepository<Author>().FindByIdAsync(id); ;
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Admin/Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(author);
                await _UW.BaseRepository<Author>().Create(author);
                //await _context.SaveChangesAsync();
                await _UW.Commit();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Admin/Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var author = await _context.Authors.FindAsync(id);
            var author = await _UW.BaseRepository<Author>().FindByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Admin/Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (id != author.AuthorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(author);
                    _UW.BaseRepository<Author>().Update(author);
                    //await _context.SaveChangesAsync();
                    await _UW.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if ( await _UW.BaseRepository<Author>().FindByIdAsync(author.AuthorID) == null)
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
            return View(author);
        }

        // GET: Admin/Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var author = await _context.Authors
            //    .FirstOrDefaultAsync(m => m.AuthorID == id);
            var author = await _UW.BaseRepository<Author>().FindByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Admin/Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var author = await _context.Authors.FindAsync(id);
            var author = await _UW.BaseRepository<Author>().FindByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            else
            {
                //_context.Authors.Remove(author);
                _UW.BaseRepository<Author>().Delete(author);
                await _UW.Commit();
                return RedirectToAction(nameof(Index));
            }
        }

        private List<EntityStates> DisplayStates(IEnumerable<EntityEntry> entities)
        {
            List <EntityStates> EntityStates= new List<EntityStates>();
            foreach(var entry in entities)
            {
                EntityStates states = new EntityStates()
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityState = entry.State.ToString(),
                };
                EntityStates.Add(states);
            }
            return EntityStates;
        }

        public async Task<IActionResult> AuthorBook(int id)
        {
            //var Authors = _context.Authors.Where(a => a.AuthorID == id).FirstOrDefaultAsync();
            var Authors = _UW.BaseRepository<Author>().FindByIdAsync(id);
            if (Authors == null)
            {
                return NotFound();
            }
            else
            {
                return View(await Authors);
            }
            //return View();
        }
    }
}
