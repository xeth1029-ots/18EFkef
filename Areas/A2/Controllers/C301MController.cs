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
using log4net;
using System.IO;
using System.Collections.Generic;

namespace WKEFSERVICE.Areas.A2.Controllers
{
    public class C301MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            C301MViewModel model = new C301MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C301MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            ActionResult rtn = View("Index", form);
            form.LoginAccount = sm.UserInfo.UserNo;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.Grid = dao.QueryC301M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        [HttpGet]
        public ActionResult Detail(string Seq)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            C301MViewModel model = new C301MViewModel();
            model.Detail = new C301MDetailModel();
            model.Detail = dao.DetailC301M(Seq.TONotNullString(), sm.UserInfo.UserNo);
            if (model.Detail == null) return new HttpNotFoundResult();
            model.Detail.Attacheds = dao.DetailAttachedsC301M(Seq.TONotNullString());
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult SaveSignUp(C301MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            var isSigned = dao.GetRowList(new MeetingSignUp() { MeetingSeq = model.Detail.Seq, TeacherAccount = sm.UserInfo.UserNo }).Any();
            if (isSigned)
            {
                sm.LastErrorMessage = "您已報名成功！<br />請於：會議報名管理 > 已報名會議管理<br />查看您的報名狀況。";
            }
            else
            {
                // 新增報名紀錄
                var Ins1 = new MeetingSignUp()
                {
                    Seq = null,
                    MeetingSeq = model.Detail.Seq,
                    TeacherAccount = sm.UserInfo.UserNo,
                    SignUpDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                    SignUpType = "1"
                };
                int rtn1 = dao.Insert(Ins1);
                // 新增出席紀錄
                var Ins2 = new MeetingAttend()
                {
                    Seq = null,
                    MeetingSeq = model.Detail.Seq,
                    TeacherAccount = sm.UserInfo.UserNo,
                    Attend = "N",
                    TestPassed = "N"
                };
                int rtn2 = dao.Insert(Ins2);
                sm.LastResultMessage = "您已報名成功！<br />請於：會議報名管理 > 報名查詢及取消<br />查看您的報名狀況。";

                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, ModifyState.SUCCESS, "A2/C301M", Ins1, null, "[MeetingSignUp]");
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, ModifyState.SUCCESS, "A2/C301M", Ins2, null, "[MeetingAttend]");
            }
            return Detail(model.Detail.Seq.TONotNullString());
        }
    }
}