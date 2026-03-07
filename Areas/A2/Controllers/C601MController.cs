using System;
using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.A2.Models;
using WKEFSERVICE.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using System.Linq;
using Omu.ValueInjecter;
using WKEFSERVICE.Services;
using Turbo.Commons;
using System.Web;
using System.IO;
using log4net;
using System.Collections.Generic;

namespace WKEFSERVICE.Areas.A2.Controllers
{
    public class C601MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log

        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C601MViewModel model = new C601MViewModel();
            return Index(model.Form);
            //return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C601MFormModel form)
        {
            A2DAO dao = new A2DAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.Grid = dao.QueryC601M(form);
                // 取明細的附檔明細
                if (form.Grid != null && form.Grid.Any())
                {
                    foreach (var row in form.Grid)
                    {
                        row.GridAttached = dao.QueryC601MAttached(row);
                    }
                    // 再重算一次分頁資訊 Session，
                    // 因為 QueryC601MAttached 內的 QueryForListAll 函數會改寫分頁 Session 資料...
                    var tmpObj = dao.QueryC601M(form);
                }
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }
    }
}