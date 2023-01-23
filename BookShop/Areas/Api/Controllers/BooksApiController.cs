using BookShop.Models;
using BookShop.Models.UnitOfWork;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly IUnitOfWork _UW;
        public BooksApiController(IUnitOfWork UW)
        {
            _UW = UW;
        }
        [HttpGet]
        public List<BooksIndexViewModel> GetBooks()
        {
            return _UW.BookRepository.GetAllBooks("","","","","","","");
        }

        [HttpPost]
        public async Task<string> CreateBook(BooksCreateEditViewModel ViewModel)
        {
            if (await _UW.BookRepository.CreateBookAsync(ViewModel))
                return "عملیات با موفقیت انجام شد";
            else
                return "در انجام عملیات خطایی رخ داده است";
        }
        
        [HttpPut]
        public async Task<string> EditBook(BooksCreateEditViewModel ViewModel)
        {
            if (await _UW.BookRepository.EditBookAsync(ViewModel))
                return "ذخیره تغییرات با موفقیت انجام شد";
            else
                return "در انجام عملیات خطایی رخ داده است";
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (await _UW.BookRepository.DeleteBookAsync(id))
                return Content("حذف کتاب با موفقیت انجام شد");
            else
                return NotFound();

        }
    }
}
