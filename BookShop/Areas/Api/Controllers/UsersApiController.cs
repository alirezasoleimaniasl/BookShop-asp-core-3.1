using BookShop.Areas.Identity.Data;
using BookShop.Classes;
using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IUsersRepository _usersRepository;
        public UsersApiController(IApplicationUserManager userManager, IApplicationRoleManager roleManager, IConvertDate convertDate, IUsersRepository usersRepository)
        {
            _userManager = userManager;
            _usersRepository = usersRepository;
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

        [HttpPost("[action]")]
        public async Task<JsonResult> Register(RegisterBaseViewModel ViewModel)
        {
            var result = await _usersRepository.RegisterAsync(ViewModel);
            if (result.Succeeded)
            {
                return new JsonResult("عضویت با موفقیت انجام شد");
            }
            else
            {
                return new JsonResult(result.Errors);
            }
        }

        [HttpPost("[action]")]
        public async Task<string> SignIn(SignInBaseViewModel ViewModel)
        {
            var User = await _userManager.FindByNameAsync(ViewModel.UserName);
            if (User == null)
                return "کاربری با این ایمیل یافت نشد";
            else
            {
                var result = await _userManager.CheckPasswordAsync(User,ViewModel.Password);
                if (result)
                    return "احراز هویت با موفقیت انجام شد";
                else
                    return "نام کاربری یا کلمه عبور شما صحیح نمی باشد";
            }

        }
    }
}
