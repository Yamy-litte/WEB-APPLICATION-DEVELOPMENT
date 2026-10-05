using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RestaurantWebsite.Filters;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // =====================================================
        // GET: Menu
        // =====================================================
        [HttpGet]
        public ActionResult Index(int? categoryId, string search, int? dishId)
        {
            // -------------------------------------------------
            // LOAD CATEGORIES
            // -------------------------------------------------

            var categories = db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToList();


            // -------------------------------------------------
            // NORMALIZE SEARCH
            // -------------------------------------------------

            search = (search ?? string.Empty).Trim();


            // -------------------------------------------------
            // LOAD DISHES
            // -------------------------------------------------

            var dishesQuery = db.Dishes
                .Include(d => d.Category)
                .Include(d => d.DishImages)
                .Where(d => d.Status == "Available");


            // Filter by category when categoryId is provided
            if (categoryId.HasValue)
            {
                dishesQuery = dishesQuery
                    .Where(d => d.CategoryId == categoryId.Value);
            }


            // Search dish by Name / Description
            if (!string.IsNullOrEmpty(search))
            {
                dishesQuery = dishesQuery
                    .Where(d =>
                        d.Name.Contains(search) ||
                        (d.Description != null &&
                         d.Description.Contains(search)));
            }


            var dishes = dishesQuery
                .OrderBy(d => d.Name)
                .ToList();


            // -------------------------------------------------
            // LOAD ACTIVE COMBOS
            // -------------------------------------------------

            var now = DateTime.Now;

            var combosQuery = db.Combos
                .Include(c =>
                    c.ComboItems.Select(ci => ci.Dish))
                .Where(c =>
                    c.IsActive &&
                    (!c.StartDate.HasValue ||
                     c.StartDate.Value <= now) &&
                    (!c.EndDate.HasValue ||
                     c.EndDate.Value >= now));


            // Search combo by Name / Description
            if (!string.IsNullOrEmpty(search))
            {
                combosQuery = combosQuery
                    .Where(c =>
                        c.Name.Contains(search) ||
                        (c.Description != null &&
                         c.Description.Contains(search)));
            }


            var combos = combosQuery
                .OrderBy(c => c.Name)
                .ToList();


            // -------------------------------------------------
            // SEND DATA TO VIEW
            // -------------------------------------------------

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.Search = search;
            ViewBag.Combos = combos;
            ViewBag.OpenDishId = dishId;


            return View("~/Views/Customer/Menu/Index.cshtml", dishes);
        }


        // =====================================================
        // GET: Menu/Details/5
        // =====================================================
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Dish ID was not provided.");
            }

            var dish = db.Dishes
                .Include(d => d.Category)
                .Include(d => d.DishImages)
                .FirstOrDefault(d => d.Id == id.Value);

            if (dish == null)
            {
                return HttpNotFound("Dish was not found.");
            }

            var reviews = db.Reviews
                .Include(r => r.User)
                .Where(r =>
                    r.DishId == id.Value &&
                    r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.DishReviews = reviews;
            ViewBag.DishReviewCount = reviews.Count;
            ViewBag.DishAverageRating =
                reviews.Count > 0
                    ? reviews.Average(r => r.Rating)
                    : 0d;

            bool canReview = false;

            if (Session["UserId"] != null)
            {
                int userId = (int)Session["UserId"];

                bool alreadyReviewed = db.Reviews.Any(r =>
                    r.UserId == userId &&
                    r.DishId == id.Value);

                canReview = !alreadyReviewed;
            }

            ViewBag.CanReview = canReview;

            ViewBag.CanReview = canReview;

            return PartialView("~/Views/Customer/Menu/Details.cshtml", dish);
        }

        // =====================================================
        // POST: Menu/AddToCart
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult AddToCart(int id, int quantity)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = Url.Action(
                            "Index",
                            "Menu")
                    });
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            if (quantity > 100)
            {
                quantity = 100;
            }

            var dish = db.Dishes
                .FirstOrDefault(d =>
                    d.Id == id &&
                    d.Status == "Available");

            if (dish == null)
            {
                TempData["CartError"] =
                    "This dish is currently unavailable.";

                return RedirectToAction("Index");
            }

            var cart = GetCart();

            if (cart.ContainsKey(id))
            {
                cart[id] += quantity;
            }
            else
            {
                cart.Add(id, quantity);
            }

            if (cart[id] > 100)
            {
                cart[id] = 100;
            }

            SaveCart(cart);

            // A cart change should allow fresh combo suggestions.
            Session.Remove("DismissedComboSuggestionSignature");

            TempData["CartSuccess"] =
                dish.Name + " has been added to your cart.";

            return RedirectToAction("Cart");
        }


        // =====================================================
        // POST: Menu/AddComboToCart
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult AddComboToCart(int id, int quantity)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = Url.Action(
                            "Index",
                            "Combo")
                    });
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            if (quantity > 100)
            {
                quantity = 100;
            }

            var now = DateTime.Now;

            var combo = db.Combos
                .FirstOrDefault(c =>
                    c.Id == id &&
                    c.IsActive &&
                    (!c.StartDate.HasValue || c.StartDate.Value <= now) &&
                    (!c.EndDate.HasValue || c.EndDate.Value >= now));

            if (combo == null)
            {
                TempData["CartError"] =
                    "This combo is currently unavailable.";

                return RedirectToAction("Index", "Combo");
            }

            var comboCart = GetComboCart();

            if (comboCart.ContainsKey(id))
            {
                comboCart[id] += quantity;
            }
            else
            {
                comboCart.Add(id, quantity);
            }

            if (comboCart[id] > 100)
            {
                comboCart[id] = 100;
            }

            SaveComboCart(comboCart);

            TempData["CartSuccess"] =
                combo.Name + " has been added to your cart.";

            return RedirectToAction("Cart");
        }


        // =====================================================
        // GET: Menu/Cart
        // =====================================================
        [HttpGet]
        [RoleAuthorize("Customer")]
        public ActionResult Cart()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = Url.Action(
                            "Cart",
                            "Menu")
                    });
            }

            var cart = GetCart();
            var comboCart = GetComboCart();

            // -------------------------------------------------
            // LOAD DISHES
            // -------------------------------------------------

            var dishes = new List<Dish>();

            if (cart.Count > 0)
            {
                var dishIds = cart.Keys.ToList();

                dishes = db.Dishes
                    .Include(d => d.Category)
                    .Include(d => d.DishImages)
                    .Where(d => dishIds.Contains(d.Id))
                    .ToList();

                var existingIds = dishes
                    .Select(d => d.Id)
                    .ToList();

                var invalidIds = cart.Keys
                    .Where(id => !existingIds.Contains(id))
                    .ToList();

                foreach (var invalidId in invalidIds)
                {
                    cart.Remove(invalidId);
                }

                SaveCart(cart);
            }


            // -------------------------------------------------
            // LOAD COMBOS
            // -------------------------------------------------

            var combos = new List<Combo>();

            if (comboCart.Count > 0)
            {
                var comboIds = comboCart.Keys.ToList();

                combos = db.Combos
                    .Include(c => c.ComboItems.Select(ci => ci.Dish))
                    .Where(c => comboIds.Contains(c.Id))
                    .ToList();

                var existingComboIds = combos
                    .Select(c => c.Id)
                    .ToList();

                var invalidComboIds = comboCart.Keys
                    .Where(id => !existingComboIds.Contains(id))
                    .ToList();

                foreach (var invalidComboId in invalidComboIds)
                {
                    comboCart.Remove(invalidComboId);
                }

                SaveComboCart(comboCart);
            }


            // -------------------------------------------------
            // CALCULATE TOTAL
            // -------------------------------------------------

            decimal cartTotal = 0m;

            foreach (var dish in dishes)
            {
                if (cart.ContainsKey(dish.Id))
                {
                    cartTotal +=
                        dish.Price * cart[dish.Id];
                }
            }

            foreach (var combo in combos)
            {
                if (comboCart.ContainsKey(combo.Id))
                {
                    cartTotal +=
                        combo.Price * comboCart[combo.Id];
                }
            }


            // -------------------------------------------------
            // COMBO SUGGESTIONS
            // -------------------------------------------------

            var suggestions =
                GetComboSuggestions(cart);

            string signature =
                BuildCartSignature(cart);

            string dismissedSignature =
                Session["DismissedComboSuggestionSignature"]
                as string;

            if (signature == dismissedSignature)
            {
                suggestions.Clear();
            }


            ViewBag.CartQuantities = cart;
            ViewBag.CartCombos = combos;
            ViewBag.ComboQuantities = comboCart;
            ViewBag.CartTotal = cartTotal;
            ViewBag.ComboSuggestions = suggestions;


            return View("~/Views/Customer/Cart/Index.cshtml", dishes);
        }


        // =====================================================
        // POST: Menu/ApplyComboSuggestion
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult ApplyComboSuggestion(int comboId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var cart = GetCart();

            var now = DateTime.Now;

            var combo = db.Combos
                .Include(c => c.ComboItems)
                .FirstOrDefault(c =>
                    c.Id == comboId &&
                    c.IsActive &&
                    (!c.StartDate.HasValue || c.StartDate.Value <= now) &&
                    (!c.EndDate.HasValue || c.EndDate.Value >= now));

            if (combo == null)
            {
                TempData["CartError"] =
                    "The selected combo is no longer available.";

                return RedirectToAction("Cart");
            }

            if (combo.ComboItems == null ||
                !combo.ComboItems.Any())
            {
                TempData["CartError"] =
                    "This combo has no items.";

                return RedirectToAction("Cart");
            }

            // Group duplicate dishes inside the same combo.
            var requirements = combo.ComboItems
                .GroupBy(ci => ci.DishId)
                .Select(g => new
                {
                    DishId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();


            // -------------------------------------------------
            // Determine maximum number of combos possible
            // -------------------------------------------------

            int comboQuantity = int.MaxValue;

            foreach (var requirement in requirements)
            {
                int availableQuantity = 0;

                if (cart.ContainsKey(requirement.DishId))
                {
                    availableQuantity =
                        cart[requirement.DishId];
                }

                int possibleQuantity =
                    availableQuantity /
                    requirement.Quantity;

                if (possibleQuantity < comboQuantity)
                {
                    comboQuantity = possibleQuantity;
                }
            }


            if (comboQuantity <= 0 ||
                comboQuantity == int.MaxValue)
            {
                TempData["CartError"] =
                    "The selected dishes are not enough to form this combo.";

                return RedirectToAction("Cart");
            }


            // OrderItem Quantity supports max 100.
            if (comboQuantity > 100)
            {
                comboQuantity = 100;
            }


            // -------------------------------------------------
            // CONSUME DISHES
            // -------------------------------------------------

            foreach (var requirement in requirements)
            {
                int usedQuantity =
                    requirement.Quantity *
                    comboQuantity;

                cart[requirement.DishId] -=
                    usedQuantity;

                if (cart[requirement.DishId] <= 0)
                {
                    cart.Remove(requirement.DishId);
                }
            }


            SaveCart(cart);


            // -------------------------------------------------
            // ADD COMBO
            // -------------------------------------------------

            var comboCart = GetComboCart();

            if (comboCart.ContainsKey(combo.Id))
            {
                comboCart[combo.Id] += comboQuantity;
            }
            else
            {
                comboCart.Add(
                    combo.Id,
                    comboQuantity);
            }

            if (comboCart[combo.Id] > 100)
            {
                comboCart[combo.Id] = 100;
            }

            SaveComboCart(comboCart);

            Session.Remove("DismissedComboSuggestionSignature");

            TempData["CartSuccess"] =
                combo.Name +
                " x " +
                comboQuantity +
                " has been created from your dishes.";

            return RedirectToAction("Cart");
        }


        // =====================================================
        // POST: Menu/DismissComboSuggestions
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult DismissComboSuggestions()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var cart = GetCart();

            Session["DismissedComboSuggestionSignature"] =
                BuildCartSignature(cart);

            return RedirectToAction("Cart");
        }


        // =====================================================
        // POST: Menu/UpdateCartItem
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult UpdateCartItem(int id, int quantity)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var cart = GetCart();

            if (!cart.ContainsKey(id))
            {
                return RedirectToAction("Cart");
            }

            if (quantity <= 0)
            {
                cart.Remove(id);
            }
            else
            {
                if (quantity > 100)
                {
                    quantity = 100;
                }

                var dish = db.Dishes
                    .FirstOrDefault(d =>
                        d.Id == id &&
                        d.Status == "Available");

                if (dish != null)
                {
                    cart[id] = quantity;
                }
                else
                {
                    cart.Remove(id);
                }
            }

            SaveCart(cart);

            Session.Remove("DismissedComboSuggestionSignature");

            return RedirectToAction("Cart");
        }


        // =====================================================
        // POST: Menu/UpdateComboCartItem
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult UpdateComboCartItem(
            int id,
            int quantity)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var comboCart = GetComboCart();

            if (!comboCart.ContainsKey(id))
            {
                return RedirectToAction("Cart");
            }

            if (quantity <= 0)
            {
                comboCart.Remove(id);
            }
            else
            {
                if (quantity > 100)
                {
                    quantity = 100;
                }

                var comboExists = db.Combos
                    .Any(c => c.Id == id);

                if (comboExists)
                {
                    comboCart[id] = quantity;
                }
                else
                {
                    comboCart.Remove(id);
                }
            }

            SaveComboCart(comboCart);

            return RedirectToAction("Cart");
        }


        // =====================================================
        // POST: Menu/RemoveFromCart
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult RemoveFromCart(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var cart = GetCart();

            if (cart.ContainsKey(id))
            {
                cart.Remove(id);
            }

            SaveCart(cart);

            Session.Remove("DismissedComboSuggestionSignature");

            return RedirectToAction("Cart");
        }


        // =====================================================
        // POST: Menu/RemoveComboFromCart
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult RemoveComboFromCart(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var comboCart = GetComboCart();

            if (comboCart.ContainsKey(id))
            {
                comboCart.Remove(id);
            }

            SaveComboCart(comboCart);

            return RedirectToAction("Cart");
        }


        // =====================================================
        // POST: Menu/ClearCart
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Customer")]
        public ActionResult ClearCart()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            Session.Remove("Cart");
            Session.Remove("ComboCart");
            Session.Remove("DismissedComboSuggestionSignature");

            return RedirectToAction("Cart");
        }


        // =====================================================
        // DISH CART
        // =====================================================

        private Dictionary<int, int> GetCart()
        {
            var cart =
                Session["Cart"] as Dictionary<int, int>;

            if (cart == null)
            {
                cart =
                    new Dictionary<int, int>();

                Session["Cart"] = cart;
            }

            return cart;
        }


        private void SaveCart(
            Dictionary<int, int> cart)
        {
            Session["Cart"] = cart;
        }


        // =====================================================
        // COMBO CART
        // =====================================================

        private Dictionary<int, int> GetComboCart()
        {
            var comboCart =
                Session["ComboCart"]
                as Dictionary<int, int>;

            if (comboCart == null)
            {
                comboCart =
                    new Dictionary<int, int>();

                Session["ComboCart"] =
                    comboCart;
            }

            return comboCart;
        }


        private void SaveComboCart(
            Dictionary<int, int> comboCart)
        {
            Session["ComboCart"] =
                comboCart;
        }


        // =====================================================
        // COMBO SUGGESTION LOGIC
        // =====================================================

        private List<Combo> GetComboSuggestions(
            Dictionary<int, int> cart)
        {
            if (cart == null ||
                cart.Count == 0)
            {
                return new List<Combo>();
            }

            var now = DateTime.Now;

            var combos = db.Combos
                .Include(c =>
                    c.ComboItems.Select(ci => ci.Dish))
                .Where(c =>
                    c.IsActive &&
                    (!c.StartDate.HasValue ||
                     c.StartDate.Value <= now) &&
                    (!c.EndDate.HasValue ||
                     c.EndDate.Value >= now))
                .OrderBy(c => c.Name)
                .ToList();


            var suggestions = new List<Combo>();

            foreach (var combo in combos)
            {
                if (combo.ComboItems == null ||
                    !combo.ComboItems.Any())
                {
                    continue;
                }

                var requirements =
                    combo.ComboItems
                        .GroupBy(ci => ci.DishId)
                        .Select(g => new
                        {
                            DishId = g.Key,
                            Quantity =
                                g.Sum(x => x.Quantity)
                        })
                        .ToList();

                bool matches = true;

                foreach (var requirement in requirements)
                {
                    if (!cart.ContainsKey(
                            requirement.DishId))
                    {
                        matches = false;
                        break;
                    }

                    if (cart[requirement.DishId] <
                        requirement.Quantity)
                    {
                        matches = false;
                        break;
                    }

                    var dish = db.Dishes
                        .FirstOrDefault(d =>
                            d.Id ==
                            requirement.DishId &&
                            d.Status == "Available");

                    if (dish == null)
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    suggestions.Add(combo);
                }
            }

            return suggestions;
        }


        private string BuildCartSignature(
            Dictionary<int, int> cart)
        {
            if (cart == null ||
                cart.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(
                "|",
                cart
                    .OrderBy(x => x.Key)
                    .Select(x =>
                        x.Key +
                        ":" +
                        x.Value));
        }


        // =====================================================
        // POST: Menu/AddReview
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(
            int dishId,
            int rating,
            string comment)
        {
            if (Session["UserId"] == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Please log in before writing a review."
                });
            }

            int userId = (int)Session["UserId"];

            var user = db.Users.FirstOrDefault(u =>
                u.Id == userId &&
                u.IsActive &&
                u.Role == "Customer");

            if (user == null)
            {
                Session.Clear();
                Session.Abandon();

                return Json(new
                {
                    success = false,
                    message = "Your account is no longer available. Please log in again."
                });
            }

            if (rating < 1 || rating > 5)
            {
                return Json(new
                {
                    success = false,
                    message = "Please select a rating from 1 to 5 stars."
                });
            }

            comment = string.IsNullOrWhiteSpace(comment)
                ? null
                : comment.Trim();

            if (comment != null && comment.Length > 1000)
            {
                return Json(new
                {
                    success = false,
                    message = "Your review cannot exceed 1000 characters."
                });
            }

            bool dishExists = db.Dishes.Any(d => d.Id == dishId);

            if (!dishExists)
            {
                return Json(new
                {
                    success = false,
                    message = "The selected dish could not be found."
                });
            }

            

            bool alreadyReviewed = db.Reviews.Any(r =>
                r.UserId == userId &&
                r.DishId == dishId);

            if (alreadyReviewed)
            {
                return Json(new
                {
                    success = false,
                    message = "You have already reviewed this dish for this order."
                });
            }

            db.Reviews.Add(new Review
            {
                UserId = userId,
                DishId = dishId,
                OrderId = null,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now,
                IsApproved = true
            });

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Your review has been submitted successfully."
            });
        }

        // =====================================================
        // DISPOSE
        // =====================================================

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}