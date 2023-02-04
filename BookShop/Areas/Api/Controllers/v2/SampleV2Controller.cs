using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Api.Controllers.v2
{
    [Route("api/[controller]")]// Add api verion in query or header
    [ApiController]
    [ApiVersion("2.0")]//http://localhost:43636/api/SampleV1?api-version=1.0
    public class SampleV2Controller : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1 from version 2", "value2 from version 2" };
        }
    }
}
