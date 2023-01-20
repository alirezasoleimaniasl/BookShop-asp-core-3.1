using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookShop.Models;
using BookShop.Models.UnitOfWork;

namespace BookShop.Areas.Admin.Pages.Publishers
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _UW;
        public IndexModel(IUnitOfWork UW)
        {
            _UW = UW;
        }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }
        public int Count { get; set; }
        public int PageSize { get; set; } = 3;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count,PageSize));
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public IList<Publisher> Publishers { get;set; }

        public async Task<IActionResult> OnGet()
        {
            Count = _UW.BaseRepository<Publisher>().GetCount();
            Publishers = await _UW.BaseRepository<Publisher>().GetPaginateResultAsync(CurrentPage, PageSize);

            return Page();
        }
    }
}
