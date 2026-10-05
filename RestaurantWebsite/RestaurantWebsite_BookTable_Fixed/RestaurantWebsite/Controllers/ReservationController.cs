using System;
using System.Linq;
using System.Web.Mvc;
using RestaurantWebsite.Filters;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // =====================================================
        // GET: Reservation/Index
        // =====================================================
        [HttpGet]
        [RoleAuthorize("Customer")]
        public ActionResult Index()
        {
            var now = DateTime.Now;
            var model = new Reservation
            {
                ReservationDate = now.AddHours(1),
                PartySize = 2
            };

            int? userId = Session["UserId"] != null
                ? (int?)Session["UserId"]
                : null;

            LoadReservationData(userId);
            ViewBag.MinReservationDate = now.AddMinutes(30).ToString("yyyy-MM-ddTHH:mm");
            ViewBag.MaxReservationDate = now.AddDays(30).ToString("yyyy-MM-ddTHH:mm");

            return View("~/Views/Customer/Reservation/Index.cshtml", model);
        }

        // =====================================================
        // POST: Reservation/Book
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult Book(Reservation model, int tableId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = Url.Action("Index", "Reservation")
                    });
            }

            int userId = (int)Session["UserId"];

            var user = db.Users.FirstOrDefault(u =>
                u.Id == userId && u.IsActive);

            if (user == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }

            model.UserId = userId;
            model.TableId = tableId;

            var table = db.RestaurantTables.FirstOrDefault(t =>
                t.Id == tableId &&
                t.IsActive);

            if (table == null)
            {
                ModelState.AddModelError("TableId", "The selected table is not available.");
            }

            if (model.PartySize < 1 || model.PartySize > 50)
            {
                ModelState.AddModelError("PartySize", "Party size must be between 1 and 50 people.");
            }
            else if (table != null && model.PartySize > table.Capacity)
            {
                ModelState.AddModelError(
                    "PartySize",
                    "Party size exceeds the selected table capacity of " + table.Capacity + ".");
            }

            DateTime now = DateTime.Now;
            DateTime minimumDate = now.AddMinutes(30);
            DateTime maximumDate = now.AddDays(30);

            if (model.ReservationDate < minimumDate)
            {
                ModelState.AddModelError(
                    "ReservationDate",
                    "Reservation time must be at least 30 minutes from now.");
            }
            else if (model.ReservationDate > maximumDate)
            {
                ModelState.AddModelError(
                    "ReservationDate",
                    "Reservation can only be made up to 30 days in advance.");
            }

            if (!string.IsNullOrWhiteSpace(model.SpecialRequest))
            {
                model.SpecialRequest = model.SpecialRequest.Trim();
                if (model.SpecialRequest.Length > 500)
                {
                    ModelState.AddModelError(
                        "SpecialRequest",
                        "Special request cannot exceed 500 characters.");
                }
            }

            // A reservation occupies a fixed 2-hour window.
            // Do not call DateTime.AddHours() on the database column,
            // because EF6 cannot translate it to SQL.
            if (table != null && ModelState.IsValid)
            {
                DateTime requestedStart = model.ReservationDate;
                DateTime requestedEnd = requestedStart.AddHours(2);
                DateTime earliestExistingStart = requestedStart.AddHours(-2);

                bool hasOverlap = db.Reservations.Any(r =>
                    r.TableId == table.Id &&
                    r.Status != "Cancelled" &&
                    r.Status != "Rejected" &&
                    r.ReservationDate < requestedEnd &&
                    r.ReservationDate > earliestExistingStart);

                if (hasOverlap)
                {
                    ModelState.AddModelError(
                        "TableId",
                        "This table is already reserved during the selected time.");
                }
            }

            if (!ModelState.IsValid)
            {
                LoadReservationData(userId);
                ViewBag.MinReservationDate = minimumDate.ToString("yyyy-MM-ddTHH:mm");
                ViewBag.MaxReservationDate = maximumDate.ToString("yyyy-MM-ddTHH:mm");
                return View("~/Views/Customer/Reservation/Index.cshtml", model);
            }

            var reservation = new Reservation
            {
                UserId = userId,
                TableId = table.Id,
                ReservationDate = model.ReservationDate,
                PartySize = model.PartySize,
                SpecialRequest = model.SpecialRequest,
                Status = "Confirmed",
                CreatedAt = DateTime.Now
            };

            db.Reservations.Add(reservation);
            db.SaveChanges();

            TempData["ReservationSuccess"] =
                "Your table has been booked successfully.";

            return RedirectToAction("MyReservations", "Account");
        }


        // =====================================================
        // POST: Reservation/Cancel
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult Cancel(int id)
        {
            int userId = (int)Session["UserId"];

            var reservation = db.Reservations.FirstOrDefault(r =>
                r.Id == id && r.UserId == userId);

            if (reservation == null)
            {
                return HttpNotFound("Reservation was not found.");
            }

            if (reservation.Status == "Cancelled")
            {
                TempData["ReservationError"] = "This reservation has already been cancelled.";
                return RedirectToAction("MyReservations", "Account");
            }

            if (reservation.ReservationDate <= DateTime.Now)
            {
                TempData["ReservationError"] = "A reservation that has already started cannot be cancelled.";
                return RedirectToAction("MyReservations", "Account");
            }

            reservation.Status = "Cancelled";
            db.SaveChanges();

            TempData["ReservationSuccess"] = "Your reservation has been cancelled successfully.";
            return RedirectToAction("MyReservations", "Account");
        }

        private void LoadReservationData(int? userId)
        {
            User user = null;

            if (userId.HasValue)
            {
                user = db.Users.FirstOrDefault(u =>
                    u.Id == userId.Value &&
                    u.IsActive);
            }

            var availableTables = db.RestaurantTables
                .Where(t => t.IsActive)
                .OrderBy(t => t.Capacity)
                .ThenBy(t => t.TableNumber)
                .ToList();

            ViewBag.CurrentUser = user;
            ViewBag.AvailableTables = availableTables;
        }

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
