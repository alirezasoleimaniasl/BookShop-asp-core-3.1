using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Controllers
{
    public class ProductsController : Controller
    {
        [Authorize(Policy ="AtLeast18")]
        public IActionResult BannedBooks()
        {
            return View();
        }
    }
}
