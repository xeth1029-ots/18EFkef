using WKEFSERVICE.Commons;
using WKEFSERVICE.Commons.Filter;
using System.Web;
using System.Web.Mvc;

namespace WKEFSERVICE
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new CustomAuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomActionFilter());
        }
    }
}
