using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Commons.Filter;
using log4net;

namespace WKEFSERVICE.Controllers
{
    public class ErrorPageController : Controller
    {
        //protected static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: ErrorPage
        public ActionResult Index(int statusCode, string exMessage, bool isAjaxRequet)
        {
            Exception lastError = null;
            if (Session != null && Session["LastException"] != null)
            {
                lastError = (Exception)Session["LastException"];
                if (!(lastError != null && lastError is Exception))
                {
                    lastError = new Exception("Session 中沒有 LastException 資訊");
                }
            }
            else
            {
                lastError = new Exception("Session 中沒有 LastException 資訊");
            }

            /*  設定在 ErrorPage 中是否要顯示錯誤 Exception 訊息及 Stacktrace,
                但若在 localhost 執行, 則不受此設定影響, 一律顯示 Exception 訊息及 Stacktrace:
                0. 不顯示
                1. 顯示 Exception Message
                2. 顯示 Exception Message 及 Stacktrace
             * */
            string show = System.Configuration.ConfigurationManager.AppSettings["ErrorPageShowExeption"];

            if (string.IsNullOrEmpty(show)) { show = "0"; }

            //if (System.Web.HttpContext.Current.Request.IsLocal) { show = "2"; }

            ExceptionHandler handler = new ExceptionHandler(new ErrorPageController());
            string sMailBody = handler.GetErrorMsg(System.Web.HttpContext.Current, lastError);
            if (!string.IsNullOrEmpty(sMailBody)) { handler.SendMailTest(sMailBody, handler.cst_EmailtoMe); }
            if (!string.IsNullOrEmpty(exMessage)) { handler.SendMailTest(exMessage, handler.cst_EmailtoMe); }

            ViewBag.Show = show;
            ViewBag.Time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.StatusCode = statusCode;
            if ("2".Equals(show))
            {
                ViewBag.prgMessage = null;
                ViewBag.Message = exMessage;
                ViewBag.Exception = lastError;
            }
            else if ("1".Equals(show))
            {
                ViewBag.prgMessage = null;
                ViewBag.Message = exMessage;
                ViewBag.Exception = null;
            }
            else
            {
                ViewBag.prgMessage = exMessage;
                ViewBag.Message = "";
                ViewBag.Exception = null;
            }

            return View("~/Views/Shared/Error.cshtml");
        }

        /// <summary>
        /// 沒有權限訊息頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult UnAuth()
        {
            return View();
        }
    }
}