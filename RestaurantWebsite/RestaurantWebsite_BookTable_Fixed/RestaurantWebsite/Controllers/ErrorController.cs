using System.Web.Mvc;

namespace RestaurantWebsite.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View("Error404");
        }

        [HttpGet]
        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;

            return View("ServerError");
        }
    }
}