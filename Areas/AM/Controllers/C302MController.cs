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
using System.Collections;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C302MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C302MViewModel model = new C302MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C302MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("Index", form);
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

        //[HttpGet] //public ActionResult Detail(string Seq, string MF = null) //{
        //    SessionModel sm = SessionModel.Get();
        //    ModelState.Clear();
        //    AMDAO dao = new AMDAO();
        //    C302MViewModel model = new C302MViewModel() { Detail = new C302MDetailModel() };
        //    model.Detail = dao.DetailC302M_Mst(Seq.TONotNullString());
        //    model.Detail.DetailRows = dao.DetailC302M_Det(Seq.TONotNullString());
        //    ViewBag.MF = MF;
        //    return View("Detail", model); //}

        [HttpPost]
        public ActionResult DetailPost(C302MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            AMDAO dao = new AMDAO();
            C302MViewModel model = new C302MViewModel() { Detail = new C302MDetailModel() };

            int iSeq = 1;
            int ivPage = 1;
            if (!int.TryParse(form.Seq, out iSeq))
            {
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }
            form.Seq = iSeq.ToString();
            if (!int.TryParse(form.vPage, out ivPage)) { ivPage = 1; }

            model.Detail = dao.DetailC302M_Mst(form.Seq.TONotNullString());
            if (model.Detail == null)
            {
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }
            //顯示頁數由1開始
            //model.Detail.vPage = ivPage.ToString();

            int iDefPageRows = 50;
            Hashtable r = MyCommonUtil.GET_MINMAXROW(ivPage, 50);
            int iMINROW = r.ContainsKey("MINROW") ? Convert.ToInt32(r["MINROW"]) : 1;
            int iMAXROW = r.ContainsKey("MAXROW") ? Convert.ToInt32(r["MAXROW"]) : iDefPageRows;

            model.Detail.DetailRows = dao.DetailC302M_Det(iSeq, iMINROW, iMAXROW);
            ViewBag.MF = form.MeetingType;
            model.Form = form;
            model.Detail.Seq = iSeq;
            model.Detail.vPage = ivPage.ToString(); 
            //model.Detail.MeetingType = form.MeetingType;
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult Save(C302MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            AMDAO dao = new AMDAO();
            dao.SaveC302M(model);
            sm.LastResultMessage = "資料已儲存";

            var Seq = model.Detail.Seq ?? -1;
            if (Seq == -1)
            {
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }

            return DetailPost(model.Form);
            //return Detail(model.Detail.Seq.TONotNullString());
        }

        [HttpPost]
        public ActionResult Report(C302MViewModel model)
        {
            return null;
        }
    }
}