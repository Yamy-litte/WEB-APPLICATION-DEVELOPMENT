using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestaurantWebsite.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RestaurantWebsite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            EnsureReviewReplyColumns();
        }

        private void EnsureReviewReplyColumns()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.Database.ExecuteSqlCommand(@"
IF COL_LENGTH('dbo.Reviews', 'RestaurantReply') IS NULL
    ALTER TABLE dbo.Reviews ADD RestaurantReply NVARCHAR(1000) NULL;

IF COL_LENGTH('dbo.Reviews', 'RestaurantReplyAt') IS NULL
    ALTER TABLE dbo.Reviews ADD RestaurantReplyAt DATETIME NULL;
");
                }
            }
            catch
            {
                // Do not prevent application startup if the database is temporarily unavailable.
            }
        }
    }
}
