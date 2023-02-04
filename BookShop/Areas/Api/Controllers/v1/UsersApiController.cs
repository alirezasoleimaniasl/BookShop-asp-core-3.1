using BookShop.Areas.Api.Classes;
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

namespace BookShop.Areas.Api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiResultFilter]
    [ApiVersion("1.0")]
    public class UsersApiController : ControllerBase
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IUsersRepository _usersRepository;
        public UsersApiController(IApplicationUserManager userManager, /*IApplicationRoleManager roleManager, IConvertDate convertDate,*/ IUsersRepository usersRepository)
        {
            _userManager = userManager;
            _usersRepository = usersRepository;
        }

        [HttpGet]
        public virtual async Task<ApiResult<List<UsersViewModel>>> Get()
        {
            return Ok(await _userManager.GetAllUsersWithRolesAsync());
        }

        //[HttpGet("{id}")]
        [HttpGet("[action]")]
        public virtual async Task<ApiResult<List<UsersViewModel>>> Get(string id)
        {
            var User = await _userManager.FindUserWithRolesByIdAsync(id);
            if (User == null)
                return NotFound();
            else
                return Ok(User);
        }

        [HttpPost("[action]")]
        public virtual async Task<ApiResult<string>> Register(RegisterBaseViewModel ViewModel)
        {
            var result = await _usersRepository.RegisterAsync(ViewModel);
            if (result.Succeeded)
            {
                return Ok("عضویت با موفقیت انجام شد");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("[action]")]
        public virtual async Task<ApiResult<string>> SignIn(SignInBaseViewModel ViewModel)
        {
            var User = await _userManager.FindByNameAsync(ViewModel.UserName);
            if (User == null)
                return BadRequest("کاربری با این ایمیل یافت نشد");
            else
            {
                var result = await _userManager.CheckPasswordAsync(User,ViewModel.Password);
                if (result)
                    return Ok("احراز هویت با موفقیت انجام شد");
                else
                    return BadRequest("نام کاربری یا کلمه عبور شما صحیح نمی باشد");
            }

        }
    }
}
