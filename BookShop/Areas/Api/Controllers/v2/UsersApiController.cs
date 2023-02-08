using BookShop.Areas.Api.Classes;
using BookShop.Areas.Api.Services;
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
        public UsersApiController(IApplicationUserManager userManager, IUsersRepository usersRepository, IjwtService jwtService)
            : base(userManager, usersRepository,jwtService)
        {
        }

        public override Task<ApiResult> Register(RegisterBaseViewModel ViewModel)
        {
            return base.Register(ViewModel);
        }
    }
}
