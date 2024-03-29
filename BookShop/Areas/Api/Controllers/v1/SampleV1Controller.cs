﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Api.Controllers.v1
{
    // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/v{v:apiVersion}/[controller]")]//http://localhost:43636/api/v3/SampleV1   start 'v' word frist of version
    //[Route("api/{v:apiVersion}/[controller]")]
    [ApiController]
    //[ApiVersion("1.0")]//http://localhost:43636/api/SampleV1?api-version=1.0
    [ApiVersion("1", Deprecated = true)]//http://localhost:43636/api/SampleV1?api-version=1.0     deprected =true
    [ApiVersion("3")]
    public class SampleV1Controller : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var ApiVersion = HttpContext.GetRequestedApiVersion().ToString();
            return new string[] { "value1 from version 1", "value2 from version 1", ApiVersion };
        }

        [HttpGet("{name}"),MapToApiVersion("3.0")]
        public string Get(string name)
        {
            return name;
        }

        [HttpGet("[action]")]
        public int test()
        {
            int i = 0;
            int num = 78 / i;

            return num;
        }
    }
}
