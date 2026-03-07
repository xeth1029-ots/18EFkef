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
    public class C101MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log

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
            A3DAO dao = new A3DAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                if (form.OrderField.TONotNullString() == "") form.OrderField = form.OrderField_list.FirstOrDefault().Value;
                if (form.OrderField_ASC_DESC.TONotNullString() == "") form.OrderField_ASC_DESC = form.OrderField_ASC_DESC_list.FirstOrDefault().Value;
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
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            C101MViewModel model = new C101MViewModel();
            model.Detail = new C101MDetailModel();
            model.Detail.IsNew = true;
            model.Detail.IsShowSignUp = "0";
            model.Detail.UpdatedAccount = sm.UserInfo.User.USERNO;
            model.Detail.PublishedUnit = sm.UserInfo.User.UNITID;
            return View("Detail", model);
        }

        [HttpGet]
        public ActionResult Detail(string Seq)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            C101MViewModel model = new C101MViewModel();
            model.Detail = new C101MDetailModel();
            model.Detail = dao.DetailC101M(Seq.TONotNullString());
            if (model.Detail == null || model.Detail.PostTo > 2)
            {
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }
            model.Detail.Attacheds = dao.DetailAttachedsC101M(Seq.TONotNullString());
            model.Detail.IsNew = false;
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult DetailAttachedsUpload(C101MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            var file = model.Detail._UPLOAD_FILE;
            if (file == null || file.ContentLength <= 0 || string.IsNullOrWhiteSpace(file.FileName))
            {
                //sm.LastErrorMessage = "請選擇上傳檔案";
            }
            else
            {
                // 上傳檔案
                string tmpPath = Server.MapPath(new AMDAO().UploadPathC104M);  // 資料夾路徑名
                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);  // 不存在則建立
                Random tmpNum = new Random();
                string tmpFileName =
                    DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                    tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                    Path.GetExtension(file.FileName);                   // 副檔名
                file.SaveAs(tmpPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A3/C101M", file.FileName, tmpFileName);
                // 資料庫新增
                dao.BeginTransaction();
                try
                {
                    #region DB Insert
                    MsgPostAttached Ins = new MsgPostAttached();
                    Ins.Seq = null;
                    Ins.MsgPostSeq = (model.Detail.Seq.TONotNullString() == "") ? -1 : model.Detail.Seq;
                    Ins.FileNameOrg = file.FileName;
                    Ins.FileNameNew = tmpFileName;
                    Ins.FileNameType = Path.GetExtension(file.FileName);
                    Ins.UpdatedAccount = sm.UserInfo.UserNo;
                    Ins.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    int keyid = dao.Insert(Ins);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, ModifyState.SUCCESS, "A3/C101M", Ins, null, "[MsgPostAttached]");
                    dao.CommitTransaction();
                    #endregion

                    #region Model Insert
                    model.Detail._UPLOAD_FILE = null;
                    if (model.Detail.Attacheds == null) model.Detail.Attacheds = new List<C101MAttachedsModel>();
                    model.Detail.Attacheds.Add(new C101MAttachedsModel()
                    {
                        ROWID = model.Detail.Attacheds.ToCount() + 1,
                        Seq = keyid,
                        MsgPostSeq = Ins.MsgPostSeq,
                        FileNameOrg = Ins.FileNameOrg,
                        FileNameNew = Ins.FileNameNew,
                        FileNameType = Ins.FileNameType,
                        UpdatedAccount = Ins.UpdatedAccount,
                        UpdatedDatetime = Ins.UpdatedDatetime.Replace('/', '-')
                    });
                    #endregion
                    //sm.LastResultMessage = "檔案上傳成功";
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.Message, ex);
                    dao.RollBackTransaction();                    
                    if (System.IO.File.Exists(tmpPath + "/" + tmpFileName))
                    {
                        System.IO.File.Delete(tmpPath + "/" + tmpFileName);
                        FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "A3/C101M", file.FileName, tmpFileName);
                    }
                    //sm.LastErrorMessage = "檔案上傳失敗";
                }
                #region 廢棄
                //Random tmpNum = new Random();
                //string tmpFileName =
                //    DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                //    tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                //    Path.GetExtension(file.FileName);                   // 副檔名
                //model.Detail.Attacheds.Add(new C101MAttachedsModel()
                //{
                //    ROWID = model.Detail.Attacheds.ToCount() + 1,
                //    Seq = tmpNum.Next(0, 999999) * -1,  // 先隨便塞一個亂數
                //    MsgPostSeq = model.Detail.Seq,
                //    FileNameOrg = file.FileName,
                //    FileNameNew = tmpFileName,
                //    FileNameType = Path.GetExtension(file.FileName),
                //    UpdatedAccount = sm.UserInfo.UserNo,
                //    UpdatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    FILE = file,
                //    isActive = true,
                //    isNewFile = true,
                //});
                //model.Detail._UPLOAD_FILE = null;
                //sm.LastResultMessage = "檔案上傳成功";
                #endregion
            }
            return PartialView("_DetailAttachedsRows", model);
        }

        [HttpPost]
        public ActionResult Save(C101MDetailModel detail)
        {
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                if (detail.OrderBy.TONotNullString() == "") detail.OrderBy = 0;
                if (detail.Actived.TONotNullString() == "") detail.Actived = "Y";
                detail.UpdatedAccount = sm.UserInfo.UserNo;
                detail.UpdatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dao.InsertOrUpdateC101M(detail);
                sm.LastResultMessage = "資料已儲存";
            }
            else
            {
                C101MViewModel model = new C101MViewModel();
                model.Detail = detail;
                return View("Detail", model);
            }
            return (detail.IsNew) ? Index(new C101MFormModel()) : Detail(detail.Seq.TONotNullString());
        }

        [HttpPost]
        public ActionResult Delete(C101MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            A3DAO dao = new A3DAO();
            int i_Seq = 0;
            if (form.Seq_forDel.TONotNullString() == "" || !int.TryParse(form.Seq_forDel.TONotNullString(), out i_Seq))
            {
                sm.LastErrorMessage = "未選擇資料";
                return Index(form);
            }

            dao.DeleteC101M(form.Seq_forDel.TONotNullString(), sm.UserInfo.UserNo);
            sm.LastResultMessage = "已刪除";
            return Index(form);
        }
    }
}