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
using System.Web;
using System.IO;
using log4net;
using System.Collections.Generic;

namespace WKEFSERVICE.Areas.A3.Controllers
{
    public class C102MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C102MViewModel model = new C102MViewModel();
            model.Form.UnitCode = sm.UserInfo.User.UNITID;
            model.Form.UnitName = sm.UserInfo.User.UNIT_NAME;
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C102MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            ActionResult rtn = View("Index", form);
            form.UnitCode = sm.UserInfo.User.UNITID;
            form.UnitName = sm.UserInfo.User.UNIT_NAME;
            form.Grid = dao.QueryC102M(form);
            return rtn;
        }

        [HttpGet]
        public ActionResult New(string Year, string Unit)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo == null || sm.UserInfo.User.UNITID != Unit)
            {
                sm.LastErrorMessage = "無權限";
                return Index(new C102MFormModel() { Year = Year });
            }
            if (Year.TONotNullString() == "")
            {
                sm.LastErrorMessage = "請先選擇年份";
                return Index(new C102MFormModel() { Year = Year });
            }
            A3DAO dao = new A3DAO();
            var CheckSame = dao.DetailC102M(Year, Unit);
            if (CheckSame != null && (CheckSame.DateS.TONotNullString() != "" || CheckSame.DateE.TONotNullString() != ""))
            {
                sm.LastErrorMessage = "資料已存在，請選擇使用修改異動資料";
                return Index(new C102MFormModel() { Year = Year });
            }
            C102MViewModel model = new C102MViewModel();
            model.Detail = new C102MDetailModel();
            model.Detail.Year = Year;
            model.Detail.UnitCode = Unit;
            model.Detail.UnitName = new ShareCodeListModel().UNIT_All_List.Where(x => x.Value == Unit).FirstOrDefault().Text;
            model.Detail.IsNew = true;
            return View("Detail", model);
        }

        [HttpGet]
        public ActionResult Detail(string Year, string Unit)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo == null || sm.UserInfo.User.UNITID != Unit)
            {
                sm.LastErrorMessage = "無權限";
                return Index(new C102MFormModel() { Year = Year });
            }
            A3DAO dao = new A3DAO();
            C102MViewModel model = new C102MViewModel();
            model.Detail = new C102MDetailModel();
            model.Detail = dao.DetailC102M(Year, Unit);
            model.Detail.UnitName = new ShareCodeListModel().UNIT_All_List.Where(x => x.Value == Unit).FirstOrDefault().Text;
            model.Detail.IsNew = false;
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult Save(C102MViewModel form)
        {
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                try
                {
                    dao.BeginTransaction();
                    form.Detail.UpdatedAccount = sm.UserInfo.UserNo;
                    form.Detail.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    dao.SaveC102M(form);
                    dao.CommitTransaction();
                    sm.LastResultMessage = "資料已儲存";
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.Message, ex);
                    dao.RollBackTransaction();
                    sm.LastErrorMessage = "儲存發生錯誤";
                }
            }
            return (form.Detail.IsNew) ? Index(new C102MFormModel() { Year = form.Detail.Year }) : View("Detail", form);
        }
    }
}