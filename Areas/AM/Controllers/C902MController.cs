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
using log4net;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using Turbo.DataLayer;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C902MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C902MViewModel model = new C902MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C902MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                AMDAO dao = new AMDAO();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.Grid = dao.QueryC902M(form);
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
            // 管理者首頁 - 查詢 所有自我審核報告
            C902MViewModel model = new C902MViewModel();
            return Index(model.Form);
        }

        [HttpGet]
        public ActionResult Audit(string Seq)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            AMDAO dao = new AMDAO();
            C902MViewModel model = new C902MViewModel();
            model.Detail = new C902MDetailModel();
            model.Detail = dao.DetailC902M(Seq);
            if (model.Detail == null) return new HttpNotFoundResult();
            if (model.Detail.UploadDeadline.TONotNullString() == "") model.Detail.UploadDeadline = (new A2DAO()).GetUploadDeadlineStr(model.Detail.Year, model.Detail.UnitCode);
            return View("Audit", model);
        }

        /// <summary>儲存／送出審核</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(C902MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            if (model.Detail.UploadDeadline.TONotNullString() == "") model.Detail.UploadDeadline = (new A2DAO()).GetUploadDeadlineStr(model.Detail.Year, model.Detail.UnitCode);
            if (model.Detail.AuditStatus_New.TONotNullString() == "")
            {
                sm.LastErrorMessage = "請選擇狀態";
                return View("Audit", model);
            }
            // 審核程序
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                SelfReport where = new SelfReport() { Seq = model.Detail.Seq };
                SelfReport update = new SelfReport()
                {
                    AuditStatus = model.Detail.AuditStatus_New,
                    AuditReason = model.Detail.AuditReason,
                    AuditAccount = sm.UserInfo.UserNo,
                    AuditDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                };
                FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C902M", dao.QueryForObject<Hashtable>("AM.LogGet__SelfReport", where), where, "[SelfReport]: Save");
                int rtn = dao.Update(update, where);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C902M", update, where, "[SelfReport]: Save");
                sm.LastResultMessage = "資料已審核";
            }
            return Index(model.Form);
        }

        [HttpPost]
        public ActionResult UnAudit(C902MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            // 檢查 Seq
            int CheckSeq = -1;
            if (!int.TryParse(form.Seq_forUnAudit, out CheckSeq))
            {
                sm.LastErrorMessage = "非法操作！";
                return Index(form);
            }
            // 還原程序開始
            AMDAO dao = new AMDAO();
            SelfReport where = new SelfReport() { Seq = form.Seq_forUnAudit.TOInt32() };
            SelfReport update = new SelfReport() { };
            ClearFieldMap cfmModel = new ClearFieldMap();
            cfmModel.Add((SelfReport x) => x.FirstTime);
            cfmModel.Add((SelfReport x) => x.AuditStatus);
            cfmModel.Add((SelfReport x) => x.AuditReason);
            cfmModel.Add((SelfReport x) => x.AuditDatetime);
            cfmModel.Add((SelfReport x) => x.AuditAccount);
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C902M", dao.QueryForObject<Hashtable>("AM.LogGet__SelfReport", where), where, "[SelfReport]: UnAudit");
            int res = dao.Update(update, where, cfmModel);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C902M", update, where, "[SelfReport]: UnAudit");
            sm.LastResultMessage = "已還原";
            return Index(form);
        }
    }
}