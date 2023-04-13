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
        private readonly string NotFoundAuthor = "نویسنده با این مشخصات یافت نشد!!!";
        public AuthorsController(IUnitOfWork UW)
        {
            _UW = UW;
        }

        // GET: Admin/Authors
        public async Task<IActionResult> Index(int page = 1, int row = 10, string sortExpression = "FirstName", string firstName = "")
        {
            var Authors = _UW.BaseRepository<Author>().FindAllAsync();
            var PagingModel = PagingList.Create(await Authors, row, page,sortExpression,"FirstName");
            PagingModel.RouteValue = new RouteValueDictionary
            {
                {"row",row},
                {"FirstName",firstName}
            };
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
                return PartialView("_AuthorsTable",PagingModel);
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
            return PartialView("_Create");
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
                await _UW.BaseRepository<Author>().CreateAsync(author);
                await _UW.Commit();
                TempData["notification"] = "درج اطلاعات با موفقیت انجام شد";
            }
            return PartialView("_Create",author);
        }

        // GET: Admin/Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, NotFoundAuthor);
            }

            //var author = await _context.Authors.FindAsync(id);
            var author = await _UW.BaseRepository<Author>().FindByIdAsync(id);
            if (author == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, NotFoundAuthor);
            }
            return PartialView("_Edit", author);
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
                //return RedirectToAction(nameof(Index));
                TempData["notification"] = "ویرایش اطلاعات با موفقیت انجام شد";
            }
            return PartialView("_Edit", author);
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

            return PartialView("_Delete",author);
        }

        // POST: Admin/Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
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
                //return RedirectToAction(nameof(Index));
                TempData["notification"] = "حذف اطلاعات با موفقیت انجام شد";
            }
            return PartialView("_Delete",author);
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

        public IActionResult Notification()
        {
            return PartialView("_Notification", TempData["notification"]);
        }
    }
}
