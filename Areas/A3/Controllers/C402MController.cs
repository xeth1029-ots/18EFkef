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
using System.Collections;

namespace WKEFSERVICE.Areas.A3.Controllers
{
    public class C402MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C402MViewModel model = new C402MViewModel();
            model.Form.MeetingType = "2";
            model.Form.CreatedUnit = sm.UserInfo.User.UNITID;
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C402MFormModel form)
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
                form.MeetingType = "2";
                form.CreatedUnit = sm.UserInfo.User.UNITID;
                form.Grid = dao.QueryC402M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        //[HttpGet]
        //public ActionResult Detail(string Seq, string vPage)
        //{
        //    SessionModel sm = SessionModel.Get();
        //    ModelState.Clear();
        //    A3DAO dao = new A3DAO();
        //    C402MViewModel model = new C402MViewModel { Detail = new C402MDetailModel() };

        //    int iSeq = 1;
        //    int ivPage = 1;
        //    if (!int.TryParse(Seq, out iSeq))
        //    {
        //        sm.LastErrorMessage = "查無資料";
        //        return Index(model.Form);
        //    }
        //    Seq = iSeq.ToString();
        //    if (!int.TryParse(vPage, out ivPage)) { ivPage = 1; }

        //    model.Detail = dao.DetailC402M_Mst(Seq.TONotNullString());
        //    if (model.Detail == null || model.Detail.MeetingType != "2")
        //    {
        //        sm.LastErrorMessage = "查無資料";
        //        return Index(model.Form);
        //    }
        //    //顯示頁數由1開始
        //    model.Detail.vPage = ivPage.ToString();

        //    int iDefPageRows = 50;
        //    Hashtable r = MyCommonUtil.GET_MINMAXROW(ivPage, 50);
        //    int iMINROW = r.ContainsKey("MINROW") ? Convert.ToInt32(r["MINROW"]) : 1;
        //    int iMAXROW = r.ContainsKey("MAXROW") ? Convert.ToInt32(r["MAXROW"]) : iDefPageRows;
        //    model.Detail.DetailRows = dao.DetailC402M_Det(iSeq, iMINROW, iMAXROW);
        //    return View("Detail", model);
        //}


        [HttpPost]
        public ActionResult DetailPost(C402MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            A3DAO dao = new A3DAO();
            C402MViewModel model = new C402MViewModel { Detail = new C402MDetailModel() };

            int iSeq = 1;
            int ivPage = 1;
            if (!int.TryParse(form.Seq, out iSeq))
            {
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }
            form.Seq = iSeq.ToString();
            if (!int.TryParse(form.vPage, out ivPage)) { ivPage = 1; }

            model.Detail = dao.DetailC402M_Mst(form.Seq.TONotNullString());
            if (model.Detail == null || model.Detail.MeetingType != "2")
            {
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }
            //顯示頁數由1開始
            model.Detail.vPage = ivPage.ToString();

            int iDefPageRows = 50;
            Hashtable r = MyCommonUtil.GET_MINMAXROW(ivPage, 50);
            int iMINROW = r.ContainsKey("MINROW") ? Convert.ToInt32(r["MINROW"]) : 1;
            int iMAXROW = r.ContainsKey("MAXROW") ? Convert.ToInt32(r["MAXROW"]) : iDefPageRows;
            model.Detail.DetailRows = dao.DetailC402M_Det(iSeq, iMINROW, iMAXROW);
            model.Form = form;
            model.Detail.Seq = iSeq;
            model.Detail.vPage = ivPage.ToString();
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult Save(C402MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            A3DAO dao = new A3DAO();
            dao.SaveC402M(model);
            sm.LastResultMessage = "資料已儲存";

            var Seq = model.Detail.Seq ?? -1;
            if (Seq == -1)
            {
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }

            //return Detail(Seq.TONotNullString(), model.Detail.vPage);
            return DetailPost(model.Form);
        }

        [HttpPost]
        public ActionResult Report(C402MViewModel model)
        {
            return null;
        }
    }
}