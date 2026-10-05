using System.Web.Mvc;
using System.Web.Routing;

namespace RestaurantWebsite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "CustomerHome",
                url: "customer",
                defaults: new { controller = "Customer", action = "Index" }
            );

            routes.MapRoute(
                name: "CustomerAbout",
                url: "customer/about",
                defaults: new { controller = "Customer", action = "About" }
            );

            routes.MapRoute(
                name: "CustomerCart",
                url: "customer/cart",
                defaults: new { controller = "Menu", action = "Cart" }
            );

            routes.MapRoute(
                name: "CustomerProfile",
                url: "customer/profile",
                defaults: new { controller = "Account", action = "Profile" }
            );

            routes.MapRoute(
                name: "CustomerChangePassword",
                url: "customer/change-password",
                defaults: new { controller = "Account", action = "ChangePassword" }
            );

            routes.MapRoute(
                name: "CustomerOrders",
                url: "customer/orders",
                defaults: new { controller = "Account", action = "MyOrders" }
            );

            routes.MapRoute(
                name: "CustomerReservations",
                url: "customer/reservations",
                defaults: new { controller = "Account", action = "MyReservations" }
            );

            routes.MapRoute(
                name: "CustomerReviews",
                url: "customer/reviews",
                defaults: new { controller = "Account", action = "MyReviews" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
