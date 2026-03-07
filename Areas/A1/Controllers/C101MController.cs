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
using System.Collections;
using Turbo.DataLayer;

namespace WKEFSERVICE.Areas.A1.Controllers
{
    public class C101MController : WKEFSERVICE.Controllers.BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            C101MViewModel model = new C101MViewModel();

            //return Index(model);
            return Index(model.Form);
        }

        /// <summary>資安檢核</summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool CheckC101MFormModelValid(C101MFormModel form)
        {
            //不應該有http字眼，有為異常
            if (!string.IsNullOrEmpty(form.TeachArea) && form.TeachArea.ToLower().Contains("http")) { return true; }
            if (!string.IsNullOrEmpty(form.TeachArea_forSQL) && form.TeachArea_forSQL.ToLower().Contains("http")) { return true; }
            //不應該有http字眼，有為異常
            if (!string.IsNullOrEmpty(form.LiveArea) && form.LiveArea.ToLower().Contains("http")) { return true; }
            if (!string.IsNullOrEmpty(form.LiveArea_forSQL) && form.LiveArea_forSQL.ToLower().Contains("http")) { return true; }
            //flag_NG = form.Industry.ToLower().Contains("http"); //if (flag_NG) { return flag_NG; }
            //flag_NG = form.Keyword.ToLower().Contains("http"); //if (flag_NG) { return flag_NG; }
            //不應該有http字眼，有為異常
            if (!string.IsNullOrEmpty(form.TeachUnit) && form.TeachUnit.ToLower().Contains("http")) { return true; }
            if (!string.IsNullOrEmpty(form.TeachUnit_forSQL) && form.TeachUnit_forSQL.ToLower().Contains("http")) { return true; }
            return false;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(C101MFormModel model)
        {
            ArrayList TeachUnitAry = new ArrayList() { "DC", "KC", "BC", "DC1", "KC1", "BC1", "DC2", "KC2", "BC2", "DC3", "KC3", "BC3" };
            if (!string.IsNullOrEmpty(model.TeachUnit) && !TeachUnitAry.Contains(model.TeachUnit)) { return new HttpNotFoundResult(); }
            ArrayList TeachUnitforSqlAry = new ArrayList() { "DC", "KC", "BC" };
            if (!string.IsNullOrEmpty(model.TeachUnit_forSQL) && !TeachUnitforSqlAry.Contains(model.TeachUnit_forSQL)) { return new HttpNotFoundResult(); }

            LOG.Debug(string.Concat("#model.ddlTeachArea: ", model.ddlTeachArea));
            LOG.Debug(string.Concat("#model.ddlLiveArea: ", model.ddlLiveArea));
            string vTeachAreaText = "";
            string vLiveAreaText = "";
            MyKeyMapDAO mkdao = new MyKeyMapDAO();
            var tmplist1 = mkdao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.City_List);
            foreach (var tmpStr in tmplist1) { if (tmpStr.CODE == model.ddlTeachArea) { vTeachAreaText = tmpStr.TEXT; break; } }
            foreach (var tmpStr in tmplist1) { if (tmpStr.CODE == model.ddlLiveArea) { vLiveAreaText = tmpStr.TEXT; break; } }
            model.TeachArea = vTeachAreaText;
            model.LiveArea = vLiveAreaText;
            LOG.Debug(string.Concat("#model.TeachArea : ", model.TeachArea));
            LOG.Debug(string.Concat("#model.LiveArea : ", model.LiveArea));

            A1DAO dao = new A1DAO();

            //C101MFormModel form = model.Form;
            ActionResult rtn = View("Index", model);
            //new HttpNotFoundResult();
            bool flag_NG = CheckC101MFormModelValid(model);
            if (flag_NG) { return new HttpNotFoundResult(); }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(model.rid, model.p);
                // 查詢結果
                model.Grid = dao.QueryC101M(model);

                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(model.rid) && model.useCache == 0) rtn = PartialView("_GridRows", model);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(model, dao, "Index");
            }
            return rtn;
        }

        /// <summary>師資介紹</summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Ipresent()
        {
            C101MViewModel model = new C101MViewModel();

            return Ipresent(model);
        }

        /// <summary>師資介紹</summary>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Ipresent(C101MViewModel model)
        {
            ArrayList TeachUnitAry = new ArrayList() { "DC", "KC", "BC", "DC1", "KC1", "BC1", "DC2", "KC2", "BC2", "DC3", "KC3", "BC3" };
            if (!string.IsNullOrEmpty(model.Form.TeachUnit) && !TeachUnitAry.Contains(model.Form.TeachUnit)) { return new HttpNotFoundResult(); }
            ArrayList TeachUnitforSqlAry = new ArrayList() { "DC", "KC", "BC" };
            if (!string.IsNullOrEmpty(model.Form.TeachUnit_forSQL) && !TeachUnitforSqlAry.Contains(model.Form.TeachUnit_forSQL)) { return new HttpNotFoundResult(); }

            LOG.Debug(string.Concat("#model.ddlTeachArea: ", model.Form.ddlTeachArea));
            LOG.Debug(string.Concat("#model.ddlLiveArea: ", model.Form.ddlLiveArea));
            string vTeachAreaText = "";
            string vLiveAreaText = "";
            MyKeyMapDAO mkdao = new MyKeyMapDAO();
            var tmplist1 = mkdao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.City_List);
            foreach (var tmpStr in tmplist1) { if (tmpStr.CODE == model.Form.ddlTeachArea) { vTeachAreaText = tmpStr.TEXT; break; } }
            foreach (var tmpStr in tmplist1) { if (tmpStr.CODE == model.Form.ddlLiveArea) { vLiveAreaText = tmpStr.TEXT; break; } }
            model.Form.TeachArea = vTeachAreaText;
            model.Form.LiveArea = vLiveAreaText;
            LOG.Debug(string.Concat("#model.TeachArea : ", model.Form.TeachArea));
            LOG.Debug(string.Concat("#model.LiveArea : ", model.Form.LiveArea));

            A1DAO dao = new A1DAO();

            ActionResult rtn = View("Ipresent", model);

            //if (ModelState.IsValid)
            //{
            //    ModelState.Clear();
            //    // 設定查詢分頁資訊
            //    dao.SetPageInfo(form.rid, form.p);
            //    // 查詢結果
            //    form.Grid = dao.QueryC101M(form);
            //    // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
            //    if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows2", model);
            //    // 設定分頁元件(_PagingLink partial view)所需的資訊
            //    base.SetPagingParams(form, dao, "Ipresent");
            //}

            model.Form.Grid = dao.QueryC101M_NewVer(model.Form);

            return rtn;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Detail(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return new HttpNotFoundResult(); }

            ModelState.Clear();

            A1DAO dao = new A1DAO();

            C101MViewModel model = new C101MViewModel();

            model.Detail = dao.DetailC101M(Seq);

            if (model.Detail == null) return new HttpNotFoundResult();

            return View("Detail", model);
        }
    }
}