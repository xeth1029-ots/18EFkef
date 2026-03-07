using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WKEFSERVICE
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("home");
            routes.IgnoreRoute("report");
            routes.IgnoreRoute("uploads");
            //string s_IgnoreRoute = "home,scripts,css,images,template,content,services,assets,report";
            //foreach (string s_IR in s_IgnoreRoute.Split(',')) {
            //    routes.IgnoreRoute(s_IR);
            //}

            //routes.MapRoute(
            //    name: "Login",
            //    url: "Login/Login",
            //    defaults: new { controller = "Login", action = "Index" },
            //    constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            //);

            //routes.MapRoute(
            //    name: "Role",
            //    url: "Role",
            //    defaults: new { controller = "Login", action = "Role" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );

            //routes.MapRoute(
            //    name: "REST",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "REST", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
