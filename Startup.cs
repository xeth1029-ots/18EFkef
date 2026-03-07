using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.MemoryStorage;
using WKEFSERVICE.Controllers;
using System.Linq;
using Hangfire.Dashboard;

[assembly: OwinStartup(typeof(WKEFSERVICE.Startup))]

namespace WKEFSERVICE
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire",
                new DashboardOptions
                {
                    Authorization = new[] { new DashboardAccessAuthFilter() },
                    //IsReadOnlyFunc = (context) =>
                    //{
                    //    // 由 DashboardContext 可識別使用者帳號、IP等
                    //    // 此處設定一律唯讀
                    //    return true;
                    //}
                });

           
        }
    }

    public class DashboardAccessAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //DashboardContext.Request 提供 Method、Path、LocalIpAddress、RemoteIpAddress 等基本屬性
            var clientIp = context.Request.RemoteIpAddress;
            //不足的話，可以轉成 OwinContext
            var owinCtx = new OwinContext(context.GetOwinEnvironment());
            var ipAddr = owinCtx.Request.LocalIpAddress;
            var isLogin = false;
            if (ipAddr == "::1" || ipAddr == "211.23.49.110" || ipAddr == "210.68.37.161" || clientIp == "::1" || clientIp == "211.23.49.110" || clientIp == "210.68.37.161")
            {
                isLogin = true;
            }
            var loginUser = owinCtx.Request.User.Identity.Name.Split('\\').Last();
            //依據來源IP、登入帳號決定可否存取
            //例如：已登入者可存取
            return isLogin;
        }
    }
}
