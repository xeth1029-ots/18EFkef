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
using System.Collections;

namespace WKEFSERVICE.Areas.A2.Controllers
{
    public class C302MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            C302MViewModel model = new C302MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C302MFormModel form)
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
                form.Grid = dao.QueryC302M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        [HttpPost]
        public ActionResult SaveUnSign(C302MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            int i_Seq = 0;
            if (form.Seq_forUnSign.TONotNullString() == "" || !int.TryParse(form.Seq_forUnSign.TONotNullString(), out i_Seq) || form.Seq_forUnSign <= 0)
            {
                sm.LastErrorMessage = "選擇有誤，請重試！";
                return Index(form);
            }

            // 取消前檢查
            var getData = dao.GetRow<MeetingAttend>(new MeetingAttend()
            {
                MeetingSeq = form.Seq_forUnSign,
                TeacherAccount = sm.UserInfo.UserNo
            });
            if (getData != null && getData.Attend == "Y")
            {
                sm.LastErrorMessage = "已出席過的會議，不可以取消報名！";
                return Index(form);
            }

            Hashtable where = new Hashtable();
            where["MeetingSeq"] = form.Seq_forUnSign;
            where["TeacherAccount"] = sm.UserInfo.UserNo;
            dao.UnSigninC302M(where);
            sm.LastResultMessage = "已取消報名";
            return Index(form);
        }

        [HttpGet]
        public ActionResult Detail(string Seq)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            C302MViewModel model = new C302MViewModel();
            model.Detail = new C302MDetailModel();
            model.Detail = dao.DetailC302M(Seq.TONotNullString(), sm.UserInfo.UserNo);
            if (model.Detail == null) return new HttpNotFoundResult();
            model.Detail.Attacheds = dao.DetailAttachedsC302M(Seq.TONotNullString());
            return View("Detail", model);
        }
    }
}