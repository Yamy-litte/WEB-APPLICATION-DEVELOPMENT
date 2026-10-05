using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RestaurantWebsite.Filters;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Controllers
{
    [RoleAuthorize("Customer")]
    public class OrderController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        private static readonly string[] AllowedOrderTypes =
        {
            "Dine-in",
            "Takeaway",
            "Delivery"
        };

        private static readonly string[] AllowedPaymentMethods =
        {
            "Cash",
            "Bank Transfer",
            "Digital Wallet"
        };

        // =====================================================
        // GET: Order/Checkout
        // =====================================================
        [HttpGet]
        public ActionResult Checkout()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = Url.Action("Checkout", "Order")
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

            var cart = GetCart();
            var comboCart = GetComboCart();

            if (cart.Count == 0 && comboCart.Count == 0)
            {
                TempData["CartError"] = "Your cart is empty.";
                return RedirectToAction("Cart", "Menu");
            }

            PrepareCheckoutData(cart, comboCart, userId);

            var order = new Order
            {
                OrderType = "Delivery",
                Phone = user.Phone,
                DeliveryAddress = user.Address
            };

            return View("~/Views/Customer/Order/Checkout.cshtml", order);
        }

        // =====================================================
        // POST: Order/PlaceOrder
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(
            Order model,
            string paymentMethod,
            string promotionCode)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
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

            var cart = GetCart();
            var comboCart = GetComboCart();

            if (cart.Count == 0 && comboCart.Count == 0)
            {
                TempData["CartError"] = "Your cart is empty.";
                return RedirectToAction("Cart", "Menu");
            }

            model.OrderType = (model.OrderType ?? "").Trim();
            model.ReservationId =
                model.ReservationId.HasValue && model.ReservationId.Value > 0
                    ? model.ReservationId
                    : (int?)null;
            model.Status = "Pending";

            ModelState.Remove("Status");

            if (!AllowedOrderTypes.Contains(model.OrderType))
            {
                ModelState.AddModelError("OrderType", "Please select a valid order type.");
            }

            if (!AllowedPaymentMethods.Contains(paymentMethod))
            {
                ModelState.AddModelError("", "Please select a valid payment method.");
            }

            model.Phone = string.IsNullOrWhiteSpace(model.Phone)
                ? user.Phone
                : model.Phone.Trim();

            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError("Phone", "Phone number is required.");
            }
            else if (model.Phone.Length > 20)
            {
                ModelState.AddModelError("Phone", "Phone number cannot exceed 20 characters.");
            }

            model.Note = string.IsNullOrWhiteSpace(model.Note)
                ? null
                : model.Note.Trim();

            if (model.Note != null && model.Note.Length > 500)
            {
                ModelState.AddModelError("Note", "Note cannot exceed 500 characters.");
            }

            // -------------------------------------------------
            // Validate order-type-specific data
            // -------------------------------------------------
            Reservation selectedReservation = null;

            if (model.OrderType == "Delivery")
            {
                model.DeliveryAddress = string.IsNullOrWhiteSpace(model.DeliveryAddress)
                    ? user.Address
                    : model.DeliveryAddress.Trim();

                model.ReservationId = null;
                model.TableId = null;

                if (string.IsNullOrWhiteSpace(model.DeliveryAddress))
                {
                    ModelState.AddModelError(
                        "DeliveryAddress",
                        "Delivery address is required for Delivery orders.");
                }
                else if (model.DeliveryAddress.Length > 500)
                {
                    ModelState.AddModelError(
                        "DeliveryAddress",
                        "Delivery address cannot exceed 500 characters.");
                }
            }
            else if (model.OrderType == "Takeaway")
            {
                model.DeliveryAddress = null;
                model.ReservationId = null;
                model.TableId = null;
            }
            else if (model.OrderType == "Dine-in")
            {
                model.DeliveryAddress = null;

                if (!model.ReservationId.HasValue)
                {
                    ModelState.AddModelError(
                        "ReservationId",
                        "Please select a confirmed table reservation for a Dine-in order.");
                }
                else
                {
                    selectedReservation = db.Reservations.FirstOrDefault(r =>
                        r.Id == model.ReservationId.Value &&
                        r.UserId == userId &&
                        r.Status == "Confirmed");

                    if (selectedReservation == null)
                    {
                        ModelState.AddModelError(
                            "ReservationId",
                            "The selected reservation is not confirmed or does not belong to you.");
                    }
                    else
                    {
                        model.TableId = selectedReservation.TableId;
                    }
                }
            }

            // -------------------------------------------------
            // Load cart data from DB and revalidate availability
            // -------------------------------------------------
            var dishIds = cart.Keys.ToList();
            var comboIds = comboCart.Keys.ToList();

            var dishes = db.Dishes
                .Include(d => d.Category)
                .Include(d => d.DishImages)
                .Where(d => dishIds.Contains(d.Id))
                .ToList();

            var combos = db.Combos
                .Include(c => c.ComboItems.Select(ci => ci.Dish))
                .Where(c => comboIds.Contains(c.Id))
                .ToList();

            if (dishes.Count != cart.Count ||
                combos.Count != comboCart.Count)
            {
                ModelState.AddModelError(
                    "",
                    "One or more items in your cart no longer exist.");
            }

            foreach (var dish in dishes)
            {
                if (dish.Status != "Available")
                {
                    ModelState.AddModelError(
                        "",
                        dish.Name + " is currently unavailable.");
                }
            }

            DateTime now = DateTime.Now;

            foreach (var combo in combos)
            {
                bool active =
                    combo.IsActive &&
                    (!combo.StartDate.HasValue || combo.StartDate.Value <= now) &&
                    (!combo.EndDate.HasValue || combo.EndDate.Value >= now);

                bool containsUnavailableDish = combo.ComboItems != null &&
                    combo.ComboItems.Any(ci =>
                        ci.Dish == null || ci.Dish.Status != "Available");

                if (!active || containsUnavailableDish)
                {
                    ModelState.AddModelError(
                        "",
                        combo.Name + " is currently unavailable.");
                }
            }

            if (!ModelState.IsValid)
            {
                PrepareCheckoutData(cart, comboCart, userId);
                return View("~/Views/Customer/Order/Checkout.cshtml", model);
            }

            // -------------------------------------------------
            // Calculate totals only from database prices
            // -------------------------------------------------
            decimal subTotal = 0m;

            foreach (var dish in dishes)
            {
                subTotal += dish.Price * cart[dish.Id];
            }

            foreach (var combo in combos)
            {
                subTotal += combo.Price * comboCart[combo.Id];
            }

            // No configurable delivery-fee field exists in the current schema.
            decimal deliveryFee = 0m;

            // -------------------------------------------------
            // Promotion
            // -------------------------------------------------
            Promotion promotion = null;
            decimal discountAmount = 0m;

            if (!string.IsNullOrWhiteSpace(promotionCode))
            {
                string code = promotionCode.Trim();

                promotion = db.Promotions.FirstOrDefault(p =>
                    p.IsActive &&
                    p.Code.ToLower() == code.ToLower());

                if (promotion == null)
                {
                    ModelState.AddModelError("", "The promotion code is invalid.");
                }
                else if (promotion.StartDate > now || promotion.EndDate < now)
                {
                    ModelState.AddModelError(
                        "",
                        "The promotion code is expired or not active yet.");
                }
                else if (subTotal < promotion.MinimumOrderAmount)
                {
                    ModelState.AddModelError(
                        "",
                        "The order does not meet the minimum amount for this promotion.");
                }
                else if (promotion.UsageLimit.HasValue &&
                         promotion.UsedCount >= promotion.UsageLimit.Value)
                {
                    ModelState.AddModelError(
                        "",
                        "This promotion has reached its usage limit.");
                }
                else
                {
                    string discountType =
                        (promotion.DiscountType ?? "").Trim().ToLower();

                    if (discountType.Contains("percent"))
                    {
                        discountAmount =
                            subTotal * promotion.DiscountValue / 100m;
                    }
                    else if (discountType.Contains("fixed") ||
                             discountType.Contains("amount"))
                    {
                        discountAmount = promotion.DiscountValue;
                    }
                    else
                    {
                        ModelState.AddModelError(
                            "",
                            "This promotion uses an unsupported discount type.");
                    }

                    if (promotion.MaximumDiscount.HasValue &&
                        discountAmount > promotion.MaximumDiscount.Value)
                    {
                        discountAmount = promotion.MaximumDiscount.Value;
                    }

                    if (discountAmount > subTotal)
                    {
                        discountAmount = subTotal;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                PrepareCheckoutData(cart, comboCart, userId);
                return View("~/Views/Customer/Order/Checkout.cshtml", model);
            }

            decimal totalAmount = subTotal - discountAmount + deliveryFee;
            if (totalAmount < 0m)
            {
                totalAmount = 0m;
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        UserId = userId,
                        ReservationId = model.OrderType == "Dine-in"
                            ? model.ReservationId
                            : null,
                        TableId = model.OrderType == "Dine-in"
                            ? model.TableId
                            : null,
                        OrderType = model.OrderType,
                        Status = "Pending",
                        DeliveryAddress = model.DeliveryAddress,
                        Phone = model.Phone,
                        SubTotal = subTotal,
                        DiscountAmount = discountAmount,
                        DeliveryFee = deliveryFee,
                        TotalAmount = totalAmount,
                        Note = model.Note,
                        CreatedAt = DateTime.Now
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    foreach (var dish in dishes)
                    {
                        int quantity = cart[dish.Id];

                        db.OrderItems.Add(new OrderItem
                        {
                            OrderId = order.Id,
                            DishId = dish.Id,
                            ComboId = null,
                            Quantity = quantity,
                            UnitPrice = dish.Price,
                            SubTotal = dish.Price * quantity,
                            Note = null
                        });
                    }

                    foreach (var combo in combos)
                    {
                        int quantity = comboCart[combo.Id];

                        db.OrderItems.Add(new OrderItem
                        {
                            OrderId = order.Id,
                            DishId = null,
                            ComboId = combo.Id,
                            Quantity = quantity,
                            UnitPrice = combo.Price,
                            SubTotal = combo.Price * quantity,
                            Note = null
                        });
                    }

                    db.SaveChanges();

                    db.Payments.Add(new Payment
                    {
                        OrderId = order.Id,
                        PaymentMethod = paymentMethod,
                        Amount = totalAmount,
                        Status = "Pending",
                        PaymentDate = DateTime.Now,
                        TransactionCode = null
                    });

                    if (promotion != null)
                    {
                        db.OrderPromotions.Add(new OrderPromotion
                        {
                            OrderId = order.Id,
                            PromotionId = promotion.Id,
                            DiscountAmount = discountAmount
                        });

                        promotion.UsedCount += 1;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    Session.Remove("Cart");
                    Session.Remove("ComboCart");
                    Session.Remove("DismissedComboSuggestionSignature");

                    TempData["OrderSuccess"] =
                        "Your order has been placed successfully.";

                    return RedirectToAction("Success", new { id = order.Id });
                }
                catch
                {
                    transaction.Rollback();

                    ModelState.AddModelError(
                        "",
                        "We could not place your order. Please try again.");

                    PrepareCheckoutData(cart, comboCart, userId);
                    return View("~/Views/Customer/Order/Checkout.cshtml", model);
                }
            }
        }

        // =====================================================
        // GET: Order/Details/5
        // =====================================================
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("MyOrders", "Account");
            }

            int userId = (int)Session["UserId"];

            var order = db.Orders
                .Include(o => o.User)
                .Include(o => o.Reservation)
                .Include(o => o.Table)
                .Include(o => o.OrderItems.Select(oi => oi.Dish))
                .Include(o => o.OrderItems.Select(oi => oi.Combo))
                .Include(o => o.Payments)
                .Include(o => o.OrderPromotions.Select(op => op.Promotion))
                .Include(o => o.Reviews)
                .FirstOrDefault(o =>
                    o.Id == id.Value &&
                    o.UserId == userId);

            if (order == null)
            {
                return HttpNotFound("Order was not found.");
            }

            var reviewedDishIds = new HashSet<int>(
                order.Reviews
                    .Where(r => r.UserId == userId && r.DishId.HasValue)
                    .Select(r => r.DishId.Value));

            bool hasExperienceReview = order.Reviews.Any(r =>
                r.UserId == userId &&
                !r.DishId.HasValue);

            ViewBag.ReviewedDishIds = reviewedDishIds;
            ViewBag.HasExperienceReview = hasExperienceReview;

            return View("~/Views/Customer/Order/Details.cshtml", order);
        }

        // =====================================================
        // POST: Order/Cancel/5
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            int userId = (int)Session["UserId"];

            var order = db.Orders.FirstOrDefault(o =>
                o.Id == id &&
                o.UserId == userId);

            if (order == null)
            {
                return HttpNotFound("Order was not found.");
            }

            if (order.Status != "Pending")
            {
                TempData["OrderError"] =
                    "You can only cancel an order while it is Pending.";

                return RedirectToAction("Details", new { id });
            }

            order.Status = "Cancelled";
            db.SaveChanges();

            TempData["OrderSuccess"] =
                "Your order has been cancelled.";

            return RedirectToAction("Details", new { id });
        }

        // =====================================================
        // POST: Order/AddExperienceReview
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExperienceReview(
            int orderId,
            int rating,
            string comment)
        {
            int userId = (int)Session["UserId"];

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

            var order = db.Orders.FirstOrDefault(o =>
                o.Id == orderId &&
                o.UserId == userId &&
                o.Status == "Delivered");

            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    message = "You can review your dining experience after the order is Delivered."
                });
            }

            bool exists = db.Reviews.Any(r =>
                r.OrderId == orderId &&
                r.UserId == userId &&
                !r.DishId.HasValue);

            if (exists)
            {
                return Json(new
                {
                    success = false,
                    message = "You have already submitted an overall review for this order."
                });
            }

            db.Reviews.Add(new Review
            {
                UserId = userId,
                DishId = null,
                OrderId = orderId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now,
                IsApproved = true
            });

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Thank you for reviewing your dining experience."
            });
        }

        // =====================================================
        // GET: Order/Success/5
        // =====================================================
        [HttpGet]
        public ActionResult Success(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("MyOrders", "Account");
            }

            int userId = (int)Session["UserId"];

            var order = db.Orders
                .Include(o => o.User)
                .Include(o => o.Reservation)
                .Include(o => o.Table)
                .Include(o => o.OrderItems.Select(oi => oi.Dish))
                .Include(o => o.OrderItems.Select(oi => oi.Combo))
                .Include(o => o.Payments)
                .FirstOrDefault(o =>
                    o.Id == id.Value &&
                    o.UserId == userId);

            if (order == null)
            {
                return HttpNotFound("Order was not found.");
            }

            return View("~/Views/Customer/Order/Success.cshtml", order);
        }

        private void PrepareCheckoutData(
            Dictionary<int, int> cart,
            Dictionary<int, int> comboCart,
            int userId)
        {
            var dishIds = cart.Keys.ToList();
            var comboIds = comboCart.Keys.ToList();

            var dishes = db.Dishes
                .Include(d => d.Category)
                .Include(d => d.DishImages)
                .Where(d => dishIds.Contains(d.Id))
                .ToList();

            var combos = db.Combos
                .Include(c => c.ComboItems.Select(ci => ci.Dish))
                .Where(c => comboIds.Contains(c.Id))
                .ToList();

            decimal subTotal = 0m;

            foreach (var dish in dishes)
            {
                if (cart.ContainsKey(dish.Id))
                {
                    subTotal += dish.Price * cart[dish.Id];
                }
            }

            foreach (var combo in combos)
            {
                if (comboCart.ContainsKey(combo.Id))
                {
                    subTotal += combo.Price * comboCart[combo.Id];
                }
            }

            var promotions = db.Promotions
                .Where(p =>
                    p.IsActive &&
                    p.StartDate <= DateTime.Now &&
                    p.EndDate >= DateTime.Now &&
                    p.MinimumOrderAmount <= subTotal &&
                    (!p.UsageLimit.HasValue ||
                     p.UsedCount < p.UsageLimit.Value))
                .OrderBy(p => p.Code)
                .ToList();

            var confirmedReservations = db.Reservations
                .Include(r => r.Table)
                .Where(r =>
                    r.UserId == userId &&
                    r.Status == "Confirmed" &&
                    r.ReservationDate >= DateTime.Now)
                .OrderBy(r => r.ReservationDate)
                .ToList();

            ViewBag.CartDishes = dishes;
            ViewBag.CartQuantities = cart;
            ViewBag.CartCombos = combos;
            ViewBag.ComboQuantities = comboCart;
            ViewBag.CheckoutSubTotal = subTotal;
            ViewBag.DeliveryFee = 0m;
            ViewBag.ActivePromotions = promotions;
            ViewBag.ConfirmedReservations = confirmedReservations;
        }

        private Dictionary<int, int> GetCart()
        {
            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart == null)
            {
                cart = new Dictionary<int, int>();
                Session["Cart"] = cart;
            }

            return cart;
        }

        private Dictionary<int, int> GetComboCart()
        {
            var comboCart = Session["ComboCart"] as Dictionary<int, int>;

            if (comboCart == null)
            {
                comboCart = new Dictionary<int, int>();
                Session["ComboCart"] = comboCart;
            }

            return comboCart;
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
