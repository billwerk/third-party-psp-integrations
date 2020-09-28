using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class InfosController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/infos")]
        public string Get()
        {
            return "Public api sample 3rd party integration with Billwerk v.1.0";
        }
    }
}