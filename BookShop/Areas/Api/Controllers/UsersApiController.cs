using BookShop.Areas.Identity.Data;
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
    public class UsersApiController : ControllerBase
    {
        private readonly IApplicationUserManager _userManager;
        public UsersApiController(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<List<UsersViewModel>> Get()
        {
            return await _userManager.GetAllUsersWithRolesAsync();
        }

        //[HttpGet("{id}")]
        [HttpGet("[action]")]
        public async Task<IActionResult> Get(string id)
        {
            var User = await _userManager.FindUserWithRolesByIdAsync(id);
            if (User == null)
                return NotFound();
            else
                return new JsonResult(User);
        }

        [HttpPost]
        public async Task<string> Register()
        {

        }
    }
}
