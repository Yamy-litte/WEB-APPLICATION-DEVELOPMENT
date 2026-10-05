using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Controllers
{
    public class ComboController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        


        // =====================================================
        // GET: Combo/Details/5
        // =====================================================
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Combo ID was not provided.");
            }

            var now = DateTime.Now;

            var combo = db.Combos
                .Include(c => c.ComboItems.Select(ci => ci.Dish))
                .FirstOrDefault(c =>
                    c.Id == id.Value &&
                    c.IsActive &&
                    (!c.StartDate.HasValue || c.StartDate.Value <= now) &&
                    (!c.EndDate.HasValue || c.EndDate.Value >= now));

            if (combo == null)
            {
                return HttpNotFound("Combo was not found.");
            }

            return PartialView("~/Views/Customer/Combo/Details.cshtml", combo);
        }


        // =====================================================
        // DISPOSE
        // =====================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}