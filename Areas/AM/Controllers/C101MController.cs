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
    public class C101MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C101MViewModel model = new C101MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C101MFormModel form)
        {
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.Grid = dao.QueryC101M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        [HttpGet]
        public ActionResult New()
        {
            C101MViewModel model = new C101MViewModel();
            model.Detail = new C101MDetailModel();
            model.Detail.IsNew = true;
            return View("Detail", model);
        }

        [HttpGet]
        public ActionResult Detail(string USERNO)
        {
            //SessionModel sm = SessionModel.Get();
            //sm.The_Previous_Url = Request.UrlReferrer.ToString();
            AMDAO dao = new AMDAO();
            C101MViewModel model = new C101MViewModel();
            model.Detail = new C101MDetailModel();
            model.Detail.IsNew = false;
            model.Detail.USERNO = USERNO;
            model.Detail = dao.GetRow(model.Detail);
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult Save(C101MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("Detail", model);

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                model.Detail.MODIP = Request.UserHostAddress;
                model.Detail.MODTIME = HelperUtil.DateTimeToLongTwString(DateTime.Now);
                model.Detail.MODUSERID = sm.UserInfo.User.USERNO;
                model.Detail.MODUSERNAME = sm.UserInfo.User.USERNAME;

                dao.BeginTransaction();
                try
                {
                    // 檢查
                    if (!CommonsServices.CheckEMail(model.Detail.EMAIL))
                    {
                        dao.RollBackTransaction();
                        ModelState.AddModelError("ErrorMessage", "電子郵件的格式錯誤！");
                        return rtn;
                    }
                    if (isCheckEMailSame(model.Detail))
                    {
                        dao.RollBackTransaction();
                        ModelState.AddModelError("ErrorMessage", "電子郵件有重複，操作失敗！");
                        return rtn;
                    }

                    if (model.Detail.IsNew)
                    {
                        // Insert
                        if (isCheckUserSame(model.Detail))
                        {
                            dao.RollBackTransaction();
                            ModelState.AddModelError("ErrorMessage", "帳號有重複，新增失敗！");
                            return rtn;
                        }
                        model.Detail.PWD = dao.EncPassword(model.Detail.USERNO);
                        model.Detail.ERRCT = 0;
                        dao.AppendC101M(model.Detail);
                        var IsSuccess = dao.SaveChangePwd_Guid(model.Detail.USERNO, model.Detail.EMAIL, Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Url.Content("~/Home/MailPasswordChange"));
                        if (!IsSuccess)
                        {
                            dao.RollBackTransaction();
                            ModelState.AddModelError("ErrorMessage", "寄信失敗！");
                            return rtn;
                        }
                        sm.LastResultMessage = "資料新增成功";
                        C101MFormModel rtnForm = new C101MFormModel();
                        rtn = Index(rtnForm);
                    }
                    else
                    {
                        // Update
                        dao.UpdateC101M(model.Detail);
                        sm.LastResultMessage = "資料更新成功";
                        //rtn = Redirect(Url.Action("Index", "C101M")/*Request.UrlReferrer.ToString()*//*sm.The_Previous_Url*/);                        
                    }
                    dao.CommitTransaction();
                    sm.RedirectUrlAfterBlock = Url.Action("Index", "C101M", new { area = "AM" });
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.Message, ex);
                    dao.RollBackTransaction();
                    throw new Exception("C101M Save failed:" + ex.Message, ex);
                }
            }
            return rtn;
        }

        private bool isCheckUserSame(C101MDetailModel model)
        {
            AMDAO dao = new AMDAO();
            var getList = dao.GetRowList<AMDBURM>(new AMDBURM() { });
            getList = getList.Where(x => x.USERNO == model.USERNO).ToList();
            if (getList.Any()) return true;
            return false;
        }

        private bool isCheckEMailSame(C101MDetailModel model)
        {
            AMDAO dao = new AMDAO();
            var getList = dao.GetRowList<AMDBURM>(new AMDBURM() { });
            getList = getList.Where(x => x.EMAIL == model.EMAIL && x.USERNO != model.USERNO).ToList();
            if (getList.Any()) return true;
            return false;
        }
    }
}