using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public RedirectResult Get()
        {
            return Redirect("/hangfire");
        }
    }
}