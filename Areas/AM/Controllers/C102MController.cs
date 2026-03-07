using System;
using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.AM.Models;
using WKEFSERVICE.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using System.Linq;
using Omu.ValueInjecter;
using WKEFSERVICE.Services;
using Turbo.Commons;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C102MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C102MViewModel model = new C102MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C102MFormModel form)
        {
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.Grid = dao.QueryC102M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        [HttpGet]
        public ActionResult SetAuth(string GRPID)
        {
            AMDAO dao = new AMDAO();
            C102MViewModel model = new C102MViewModel();
            model.SetAuth = new C102MSetAuthModel();
            model.SetAuth.GRPID = GRPID;
            model.SetAuth = dao.GetRow(model.SetAuth);
            model.SetAuth.SetAuthGrid = null;
            return View("SetAuth", model);
        }

        [HttpPost]
        public ActionResult SearchAuth(C102MViewModel model)
        {
            ModelState.Clear();
            AMDAO dao = new AMDAO();
            model.SetAuth.SetAuthGrid = dao.GetC102MAuthList(model.SetAuth);
            return View("SetAuth", model);
        }

        [HttpPost]
        public ActionResult SaveAuth(C102MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                if (model.SetAuth.SetAuthGrid != null)
                {
                    // 判斷已選取項目內的資訊是否完整
                    foreach (var item in model.SetAuth.SetAuthGrid)
                    {
                        if (string.IsNullOrEmpty(item.GRPID)) throw new ArgumentNullException("C102MSetAuthGridModel.GRPID");
                        if (string.IsNullOrEmpty(item.SYSID)) throw new ArgumentNullException("C102MSetAuthGridModel.SYS_ID");
                        if (string.IsNullOrEmpty(item.PRGID)) throw new ArgumentNullException("C102MSetAuthGridModel.PRGID");

                        // 解決PKEY不得為null
                        if (string.IsNullOrEmpty(item.MODULES)) item.MODULES = "";
                        if (string.IsNullOrEmpty(item.SUBMODULES)) item.SUBMODULES = "";

                        item.MODIP = Request.UserHostAddress;
                        item.MODTIME = HelperUtil.DateTimeToLongTwString(DateTime.Now);
                        item.MODUSERID = sm.UserInfo.User.USERNO;
                        item.MODUSERNAME = sm.UserInfo.User.USERNAME;
                    }
                    // 執行
                    AMDAO dao = new AMDAO();
                    dao.UpdateOrAppendC102M(model.SetAuth.SetAuthGrid);
                    sm.LastResultMessage = "權限設定已儲存";
                    //sm.RedirectUrlAfterBlock = Url.Action("Index", "C102M", new { area = "AM" });
                }
            }
            return View("SetAuth", model);
        }
    }
}