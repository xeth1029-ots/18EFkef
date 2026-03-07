using System;
using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.A1.Models;
using WKEFSERVICE.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using System.Linq;
using Omu.ValueInjecter;
using WKEFSERVICE.Services;
using Turbo.Commons;

namespace WKEFSERVICE.Areas.A1.Controllers
{
    public class C102MController : WKEFSERVICE.Controllers.BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            C102MViewModel model = new C102MViewModel();

            //return Index(model);
            return Index(model.Form);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(C102MFormModel model)
        {
            A1DAO dao = new A1DAO();

            //C102MFormModel form = model.Form;
            ActionResult rtn = View("Index", model);

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(model.rid, model.p);
                // 查詢結果
                model.Grid = dao.QueryC102M(model);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(model.rid) && model.useCache == 0) rtn = PartialView("_GridRows", model);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(model, dao, "Index");
            }

            return rtn;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult NewsDetail(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return new HttpNotFoundResult(); }

            ModelState.Clear();

            SessionModel sm = SessionModel.Get();

            A1DAO dao = new A1DAO();

            C102MViewModel model = new C102MViewModel();

            // 取得公告資料 // 檢查是否確實有此公告
            model.Detail = dao.DetailC102M(Seq);

            if (model.Detail == null) return new HttpNotFoundResult();

            // 檢查閱覽權限
            int LoginCharacter = (sm.UserInfo == null) ? 1 : sm.UserInfo.LoginCharacter.TOInt32();

            if (model.Detail.PostTo > LoginCharacter)
            {
                sm.LastErrorMessage = "查閱對象有誤"; //權限不足
                return Index();
            }
            if (DateTime.Parse(model.Detail.PostDateE) < DateTime.Now)
            {
                sm.LastErrorMessage = "查閱時限已過"; //權限不足
                return Index();
            }

            // 取得公告附檔資料
            model.Detail.Attacheds = dao.DetailAttachedsC102M(Seq);

            return View("Detail", model);
        }
    }
}