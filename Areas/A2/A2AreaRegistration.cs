using System.Web.Mvc;

namespace WKEFSERVICE.Areas.A2
{
    public class A2AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "A2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "A2_default",
            //    "A2/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
              "A2_default",
              "A2/{controller}/{action}",
              new { action = "Index" }
          );
        }
    }
}