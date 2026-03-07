using System.Web.Mvc;

namespace WKEFSERVICE.Areas.SHARE
{
    public class SHAREAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SHARE";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "SHARE_default",
            //    "SHARE/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
                "SHARE_default",
                "SHARE/{controller}/{action}",
                new { action = "Index" }
            );
        }
    }
}