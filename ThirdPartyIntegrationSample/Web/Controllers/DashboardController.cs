using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class DashboardController : ControllerBase
    {
        /*
        public RedirectResult Get()
        {
            return Redirect("/hangfire");
        }
        */
        
        public object Get()
        {
            return new
            {
                Message="Test"
            };
        }        
    }
}