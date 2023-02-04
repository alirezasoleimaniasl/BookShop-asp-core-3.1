using BookShop.Areas.Api.Classes;
using BookShop.Areas.Identity.Data;
using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Api.Controllers.v2
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class UsersApiController : v1.UsersApiController
    {
        public UsersApiController(IApplicationUserManager userManager, IUsersRepository usersRepository)
            : base(userManager, usersRepository)
        {
        }

        public override async Task<ApiResult<string>> Register(RegisterBaseViewModel ViewModel)
        {
            return Ok();
        }
    }
}
