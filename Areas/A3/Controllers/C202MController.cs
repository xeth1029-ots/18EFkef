using System;
using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.A3.Models;
using WKEFSERVICE.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using System.Linq;
using Omu.ValueInjecter;
using WKEFSERVICE.Services;
using Turbo.Commons;
using log4net;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Collections;

namespace WKEFSERVICE.Areas.A3.Controllers
{
    public class C202MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            C202MViewModel model = new C202MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C202MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.UnitCode = sm.UserInfo.User.UNITID;
                if (form.UnitCode == "0") form.UnitCode = null;
                form.Grid = dao.QueryC202M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        [HttpGet]
        public ActionResult AutoLoad()
        {
            // 分署首頁 - 查詢 所有自我審核報告
            C202MViewModel model = new C202MViewModel();
            return Index(model.Form);
        }

        [HttpGet]
        public ActionResult AutoLoad2()
        {
            // 分署首頁 - 查詢 未審核資料
            C202MViewModel model = new C202MViewModel();
            model.Form.AuditStatus = "S";
            return Index(model.Form);
        }

        [HttpGet]
        public ActionResult Audit(string Seq)
        {
            ModelState.Clear();
            A3DAO dao = new A3DAO();
            C202MViewModel model = new C202MViewModel();
            model.Detail = new C202MDetailModel();
            model.Detail = dao.DetailC202M(Seq);
            if (model.Detail == null) return new HttpNotFoundResult();
            return View("Audit", model);
        }

        [HttpPost]
        public ActionResult Save(C202MViewModel model)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            if (model.Detail.AuditStatus_New.TONotNullString() == "")
            {
                sm.LastErrorMessage = "請選擇狀態";
                return View("Audit", model);
            }
            // 審核程序
            ReportRecord where = new ReportRecord();
            where.Seq = model.Detail.Seq;
            ReportRecord update = new ReportRecord();
            update.AuditStatus = model.Detail.AuditStatus_New;
            update.AuditAccount = sm.UserInfo.UserNo;
            update.AuditDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C202M", dao.QueryForObject<Hashtable>("AM.LogGet__ReportRecord", where), where, "[ReportRecord]");
            int rtn = dao.Update(update, where);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C202M", update, where, "[ReportRecord]");
            sm.LastResultMessage = "資料已更新";
            return Index(model.Form);
        }
    }
}