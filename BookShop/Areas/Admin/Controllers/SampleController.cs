using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SampleController : Controller
    {
        private readonly BookShopContext _context;
        //private readonly BookRepository _booksRepository;
        public SampleController(BookShopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var query = EF.CompileAsyncQuery((BookShopContext context, int id)
                => context.Books.SingleOrDefault(b => b.BookID == id));

            for(int i=0;i<=1000;i++)
            {
                //var Book = _context.Books.SingleOrDefault(b => b.BookID == i);
                var Book = query(_context,i);
            }

            sw.Stop();
            return View(sw.ElapsedMilliseconds);
        }
    }
}
