using BookShop.Areas.Api.Classes;
using BookShop.Models;
using BookShop.Models.UnitOfWork;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Api.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [ApiResultFilter]
    public class BooksApiController : ControllerBase
    {
        private readonly IUnitOfWork _UW;
        public BooksApiController(IUnitOfWork UW)
        {
            _UW = UW;
        }
        [HttpGet]
        public ApiResult<List<BooksIndexViewModel>> GetBooks()
        {
            return Ok(_UW.BookRepository.GetAllBooks("", "", "", "", "", "", ""));
        }

        [HttpPost]
        public async Task<ApiResult> CreateBook(BooksCreateEditViewModel ViewModel)
        {
            if (await _UW.BookRepository.CreateBookAsync(ViewModel))
            {
                return Ok();
            }
            else
                return BadRequest("در انجام عملیات خطایی رخ داد");
        }

        [HttpPut]
        public async Task<ApiResult> EditBook(BooksCreateEditViewModel ViewModel)
        {
            var result = await _UW.BookRepository.EditBookAsync(ViewModel);
            if (result.IsSuccess == true)
                return Ok();
            else
                return BadRequest("در انجام عملیات خطایی رخ داد");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (await _UW.BookRepository.DeleteBookAsync(id))
                return Ok();
            else
                return NotFound();

        }
    }
}
