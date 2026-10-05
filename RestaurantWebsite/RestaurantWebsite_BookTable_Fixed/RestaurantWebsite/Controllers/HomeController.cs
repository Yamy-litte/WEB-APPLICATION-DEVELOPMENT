using System.Web.Mvc;

namespace RestaurantWebsite.Controllers
{
    public class HomeController : Controller
    {
        // Keep the default MVC URL (/) but use the same Customer Home
        // as /customer so Home never loads a different set of data.
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToRoute("CustomerHome");
        }

        [HttpGet]
        public ActionResult About()
        {
            return RedirectToRoute("CustomerAbout");
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
    }
}
