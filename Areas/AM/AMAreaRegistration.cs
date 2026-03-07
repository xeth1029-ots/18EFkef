using System.Web.Mvc;

namespace WKEFSERVICE.Areas.AM
{
    public class AMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "AM_default",
            //    "AM/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
                "AM_default",
                "AM/{controller}/{action}",
                new { action = "Index"}
            );
        }
    }
}