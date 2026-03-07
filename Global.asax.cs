using WKEFSERVICE.Controllers;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WKEFSERVICE.Commons;
using Turbo.Commons;
using WKEFSERVICE.Models;
using WKEFSERVICE.Services;
using WKEFSERVICE.DataLayers;
using System.Configuration;

namespace WKEFSERVICE
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected static readonly ILog LOG = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch
                      (new System.IO.FileInfo(Server.MapPath("~/log4net.config")));

            LOG.Info("#Application_Start");

            // 加入自定義的 View/PartialView Location 設定
            ExtendedRazorViewEngine engine = ExtendedRazorViewEngine.Instance();
            // 報表模組-客制化報表結果顯示用的 PartialViews
            engine.AddPartialViewLocationFormat("~/Views/Report/Custom/{0}.cshtml");
            engine.Register();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //加入技檢系統共用的資料 Model 綁定
            //ModelBinders.Binders.Add(typeof(ExamUnitModel), new ExamUnitModelBinder());

            //啟動 BackgroundJob
            //HangfireBootstrapper.Instance.Start();
        }

        protected void Application_EndRequest()
        {
            if (Response.Cookies == null) { return; }
            foreach (var item in Response.Cookies)
            {
                if (item != null && item.TONotNullString() != "")
                {
                    Response.Cookies[item.TONotNullString()].Secure = true;
                    Response.Cookies[item.TONotNullString()].HttpOnly = true;
                }
            }
        }

        protected void Application_Error()
        {
            // Code that runs when an unhandled error occurs
            //Exception lastError = Server.GetLastError();
            //Server.ClearError();            

            // handled the exception and route to error page   
            // ExceptionHandler handler = new ExceptionHandler(new ErrorPageController());
            //handler.RouteErrorPage(this.Context, lastError);
            //Response.StatusCode = 404;

            // Code that runs when an unhandled error occurs
            Exception lastError = Server.GetLastError();
            LOG.Error("ERROR：" + lastError.Message, lastError);
            // handled the exception and route to error page   
            ExceptionHandler handler = new ExceptionHandler(new ErrorPageController());
            string sMailBody = handler.GetErrorMsg(this.Context, lastError);
            if (!string.IsNullOrEmpty(sMailBody))
            {
                try
                {
                    handler.SendMailTest(sMailBody, handler.cst_EmailtoMe);
                }
                catch (Exception ex)
                {
                    LOG.Error("#Application_Error: " + sMailBody, ex);
                }
                LOG.Error(string.Concat("#Application_Error: ", sMailBody), lastError);
            }
            Server.ClearError();

            handler.RouteErrorPage(this.Context, lastError);
            //Response.StatusCode = 404;
            //Response.End();
        }

        /// <summary> request header info </summary>
        /// <param name="s_vk1"></param>
        /// <returns></returns>
        string Get_HeaderValue_info(string s_vk1)
        {
            //string s_vk1 = "X-Scan-Memo";
            //string s_memo = string.Empty;
            string s_rst = string.Empty;
            if (Request.Headers == null) { return s_rst; }
            if (s_vk1.Equals("-1"))
            {
                foreach (string s_k1 in Request.Headers.AllKeys)
                {
                    string[] s_amlist1 = Request.Headers.GetValues(s_k1);
                    if (s_amlist1 != null)
                    {
                        string s_rst2 = string.Empty;
                        foreach (string s_v1 in s_amlist1)
                        {
                            s_rst2 += string.Concat(((!string.IsNullOrEmpty(s_rst2)) ? ";" : ""), s_v1);
                        }
                        if (!string.IsNullOrEmpty(s_rst2))
                        {
                            s_rst += string.Concat(((!string.IsNullOrEmpty(s_rst)) ? "\n" : ""), "[", s_k1, "]=", s_rst2);
                        }
                    }
                }
            }
            else
            {
                StringComparison comp1 = StringComparison.OrdinalIgnoreCase;
                foreach (string s_k1 in Request.Headers.AllKeys)
                {
                    if (s_k1.IndexOf(s_vk1, comp1) > -1 && s_vk1.Length == s_k1.Length)
                    {
                        string[] s_amlist1 = Request.Headers.GetValues(s_k1);
                        if (s_amlist1 != null)
                        {
                            foreach (string s_v1 in s_amlist1)
                            {
                                s_rst += string.Concat(((!string.IsNullOrEmpty(s_rst)) ? ";" : ""), s_v1);
                            }
                        }
                    }
                }
            }
            //if (Request.Headers.AllKeys.Any(k => k.Equals(s_vk1))) { }
            return s_rst;
        }

        /// <summary>Insecure Deployment: HTTP Request Smuggling(11621.11622) </summary>
        /// <returns></returns>
        bool chk_HK1()
        {
            string cst_k1 = "X-Scan-Memo";
            string s_rst = Get_HeaderValue_info(cst_k1);
            if (string.IsNullOrEmpty(s_rst) || s_rst.Length == 0) { return false; }
            //StringComparison compIgnoreCase = StringComparison.OrdinalIgnoreCase;
            //if (!string.IsNullOrEmpty(s_rst) && s_rst.Length > 0) { }
            string[] EngAry = new string[] { @"Engine=""Http+Request+Smuggling""", @"Engine=""Parameter+Injection+Engine""", @"Engine=""Http+Response+Splitting""", @"Engine=""Parameter+Based+Redirection""" };
            foreach (string vkey in EngAry)
            {
                if (s_rst.IndexOf(vkey, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    LOG.Error(string.Format("##Application_BeginRequest [chk_HK1]:{0}:{1}:{2}", cst_k1, vkey, s_rst));
                    return true;
                }
            }
            //Http+Response+Splitting
            //else { LOG.Info(string.Format("##Application_BeginRequest [chk_HK1]:{0}:{1}", cst_k1, s_rst)); }
            return false;
        }

        /// <summary>HeaderValue檢核</summary>
        /// <returns></returns>
        bool chk_HK2()
        {
            //bool flag_hk2 = false;
            string s_Hostall = ConfigurationManager.AppSettings["WebFmHost1"];
            if (s_Hostall == null || string.IsNullOrEmpty(s_Hostall) || s_Hostall.Length <= 1) { return false; }

            string cst_k1 = "Host";
            string s_rst = Get_HeaderValue_info(cst_k1);
            //StringComparison compIgnoreCase = StringComparison.OrdinalIgnoreCase;
            if (!string.IsNullOrEmpty(s_rst) && s_rst.Length > 0)
            {
                //查無資料是攻擊
                if (s_Hostall.IndexOf(s_rst, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    LOG.Error(string.Format("##Application_BeginRequest [chk_HK2]:{0}:{1}", cst_k1, s_rst));
                    return true; //flag_hk2 = true;
                }
            }
            return false;
        }

        /// <summary>PagingInfo.PageSize CHECK</summary>
        /// <returns></returns>
        bool chk_HK3()
        {
            string[] PageAry = new string[] { "PagingInfo.PageSize", "PagingInfo.Total", "PagingInfo.PageIdx", "PagingInfo.TotalPages" };
            try
            {
                //string isLogRequest = System.Configuration.ConfigurationManager.AppSettings["IsLogRequest"];
                //if (isLogRequest.Trim() == "false") { return; }
                //遍歷POST參數
                if (Request.Form == null) { return false; }
                foreach (string key in Request.Form.AllKeys)
                {
                    if (string.IsNullOrEmpty(key) || key == "__VIEWSTATE" || key == "__VIEWSTATEGENERATOR" || key == "__EVENTVALIDATION") { continue; }
                    //LOG.Info(string.Concat(key, ":", Request.Form[key]));
                    foreach (string vkey in PageAry)
                    {
                        if ((key.IndexOf(vkey, StringComparison.OrdinalIgnoreCase) > -1) && vkey.Length == key.Length && !string.IsNullOrEmpty(Request.Form[key]))
                        {
                            int iVal = -1;
                            if (Request.Form[key].IndexOf(',') == -1)
                            {
                                if (!int.TryParse(Request.Form[key], out iVal))
                                {
                                    LOG.Error(string.Concat("##Application_BeginRequest chk_HK3 Form[key]!: ", key, ":", Request.Form[key]));
                                    return true;
                                }
                            }
                            else
                            {
                                foreach (string fVal in Request.Form[key].Split(','))
                                {
                                    if (!string.IsNullOrEmpty(fVal))
                                    {
                                        if (!int.TryParse(fVal, out iVal))
                                        {
                                            LOG.Error(string.Concat("##Application_BeginRequest chk_HK3 Form[key]!!: ", key, ":", Request.Form[key]));
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //遍歷GET參數
                if (Request.QueryString == null) { return false; }
                foreach (string key in Request.QueryString.AllKeys)
                {
                    if (string.IsNullOrEmpty(key)) continue;
                    //比對指定的參數
                    foreach (string vkey in PageAry)
                    {
                        if (string.IsNullOrEmpty(Request.QueryString[key]) || vkey.Length != key.Length) continue;
                        if (key.IndexOf(vkey, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            int iVal = -1;
                            if (Request.QueryString[key].IndexOf(',') == -1)
                            {
                                if (!int.TryParse(Request.QueryString[key], out iVal))
                                {
                                    LOG.Error(string.Concat("##Application_BeginRequest chk_HK3 QueryString[key]!: ", key, ":", Request.QueryString[key]));
                                    return true;
                                }
                            }
                            else
                            {
                                foreach (string fVal in Request.QueryString[key].Split(','))
                                {
                                    if (!string.IsNullOrEmpty(fVal))
                                    {
                                        if (!int.TryParse(fVal, out iVal))
                                        {
                                            LOG.Error(string.Concat("##Application_BeginRequest chk_HK3 QueryString[key]!!: ", key, ":", Request.QueryString[key]));
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //LOG.Info(string.Concat(key, ":", Request.QueryString[key]));
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                return true;
            }
            return false;
        }

        /// <summary> 每次頁面的請求都會觸發 </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(object source, EventArgs e)
        {
            //A6 Security Misconfiguration
            //A6:2017(OWASP Top 10 Application Security Risks, A6: 2017)
            //this.Response.Headers["X-Content-Type-Options"] = "nosniff";

            //string s_ck = Get_HeaderValue_info("-1");
            //LOG.Debug(string.Format("##Application_BeginRequest -1={0} ", s_ck));
            //string s_ck = Get_HeaderValue_info("COOKIE");
            //LOG.Debug(string.Format("##Application_BeginRequest COOKIE={0} ", s_ck));
            //bool flag_hk1 = false;
            //嚴重風險HTTP Request Smuggling的解決方法。
            //Insecure Deployment: HTTP Request Smuggling(11621.11622)
            //資安特殊狀況hk1控制;
            //bool flag_hk1 = chk_HK1();
            if (chk_HK1() || chk_HK2() || chk_HK3())
            {
                //Response.AppendHeader("Cache-Control", "private");
                //Response.Cache.AppendCacheExtension("no-cache, no-store, must-revalidate");
                if (Response.IsClientConnected) { Response.StatusCode = 404; }
                Response.End();
                return;
            }
        }

    }
}
