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
using System.Web;
using System.Collections;
using static WKEFSERVICE.Commons.FileHeader;

namespace WKEFSERVICE.Areas.A2.Controllers
{
    public class C501MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            C501MViewModel model = new C501MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C501MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.TeacherAccount = sm.UserInfo.UserNo;
                form.Grid = dao.QueryC501M(form);
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
            C501MViewModel model = new C501MViewModel();
            model.Detail = new C501MDetailModel();
            model.Detail.UploadDeadline = "請選擇";
            model.Detail.IsNew = true;
            return View("Detail", model);
        }

        [HttpGet]
        public ActionResult Detail(string Seq)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            C501MViewModel model = new C501MViewModel();
            model.Detail = new C501MDetailModel();
            var tmpModel = dao.GetRow(new ReportRecord() { Seq = Seq.TOInt64() });
            model.Detail.InjectFrom(tmpModel);
            model.Detail.UploadDeadline = dao.GetUploadDeadlineStr(model.Detail.Year, sm.UserInfo.User.UNITID);
            model.Detail.IsNew = false;
            return View("Detail", model);
        }

        [HttpPost]
        public ActionResult Save(C501MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            model.Detail.UploadDeadline = dao.GetUploadDeadlineStr(model.Detail.Year, sm.UserInfo.User.UNITID);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                #region 檢查是否在上傳的日期區間內
                // 檢查是否在上傳的日期區間內
                var dateUploadDeadline = dao.GetUploadDeadline(model.Detail.Year, sm.UserInfo.User.UNITID);
                if (dateUploadDeadline != null && dateUploadDeadline.Count == 2)
                {
                    DateTime DateS = DateTime.Parse(dateUploadDeadline["DateS"].TONotNullString());
                    DateTime DateE = DateTime.Parse(dateUploadDeadline["DateE"].TONotNullString());
                    if (DateTime.Now.Date < DateS || DateTime.Now.Date > DateE)
                    {
                        sm.LastErrorMessage = "已超過繳交期限，無法上傳檔案！";
                        return View("Detail", model);
                    }
                }
                #endregion
                #region 檢查檔案
                // 檢查檔案
                var file = model.Detail._UPLOAD_FILE;
                if (file != null)
                {
                    // 檢查檔案大小
                    if (file.ContentLength > 20 * 1024 * 1024)  // 位元組 *1024 *1024 = MB
                    {
                        sm.LastErrorMessage = "上傳檔案格式限定 DOC、DOCX、PPT、PDF、ODT、ODP，檔案大小以 20MB 為限。";
                        return View("Detail", model);
                    }

                    // 試做 230116
                    MemoryStream ms = new MemoryStream();
                    file.InputStream.CopyTo(ms);

                    // 檢查檔案格式
                    string[] fileArr = { ".DOC", ".DOCX", ".PPT", ".PDF", ".ODT", ".ODP" };
                    if (!fileArr.Contains(Path.GetExtension(file.FileName).ToUpper()) || !CheckFileType(ms, TypeFileHeader.Document))
                    {
                        sm.LastErrorMessage = "上傳檔案格式限定 DOC、DOCX、PPT、PDF、ODT、ODP，檔案大小以 20MB 為限。";
                        return View("Detail", model);
                    }
                    // Check OK
                    if (file.ContentLength > 0 && !string.IsNullOrWhiteSpace(file.FileName))
                    {
                        // 實際檔案上傳程序，移到函數 DoFileUpload() 呼叫使用
                    }
                }
                #endregion
                #region 檢查是否為重複上傳 & 上傳程序
                // 檢查是否為重複上傳
                // 一年度一個教師只能有一份【報告類別】= 1 之教學自我審核報告
                var getDBData = dao.GetRow(new ReportRecord()
                {
                    Year = model.Detail.Year,
                    TeacherAccount = sm.UserInfo.UserNo,
                    ReportType = "1"
                });
                if (getDBData == null)
                {
                    // 否 -> 新增一筆資料，並存入【首次上傳時間】
                    DoFileUpload(file, ref model);
                    ReportRecord InsObj = new ReportRecord();
                    InsObj.InjectFrom(model.Detail);
                    InsObj.TeacherAccount = sm.UserInfo.UserNo;
                    InsObj.ReportType = "1";
                    InsObj.FirstTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    InsObj.AuditStatus = " ";
                    InsObj.AuditAccount = null;
                    InsObj.AuditDatetime = null;
                    InsObj.UpdatedAccount = sm.UserInfo.UserNo;
                    InsObj.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    int rtn = dao.Insert<ReportRecord>(InsObj);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, ModifyState.SUCCESS, "A2/C501M", InsObj, null, "[ReportRecord]");
                }
                else
                {
                    // 是(已上傳過) ->
                    //   (1) 若【審核狀態】已為 (Y：審核通過)、(N：審核不通過)、(S：送件審核中)，不可更新。
                    //       -> 跳出提示："此份報告已送審，不可再更新。"、"此份報告已審核，不可再更新。"
                    //   (2) 若【審核狀態】已為 (R：退件修正)、(半形空格(未送出))，可更新，
                    //       -> 跳出提示："您已上傳過報告，是否確定要更新檔案資訊?"若是，update資料，但不可蓋過【首次上傳時間】。
                    string[] readonlyArr = { "S", "Y", "N" };
                    if (readonlyArr.Contains(getDBData.AuditStatus.TONotNullString()))
                    {
                        if (getDBData.AuditStatus == "S") sm.LastErrorMessage = "此份報告已送審，不可再更新。";
                        else sm.LastErrorMessage = "此份報告已審核，不可再更新。";
                        return View("Detail", model);
                    }
                    else
                    {
                        ReportRecord where = new ReportRecord();
                        where.Seq = getDBData.Seq;
                        ReportRecord update = new ReportRecord();
                        DoFileUpload(file, ref model);
                        update.Year = model.Detail.Year;
                        update.TeacherAccount = sm.UserInfo.UserNo;
                        update.ReportType = "1";
                        update.JobAbilityCode = model.Detail.JobAbilityCode;
                        update.ReportName = model.Detail.ReportName;
                        update.FileNameRemark = model.Detail.FileNameRemark;
                        update.FileNameOrg = model.Detail.FileNameOrg;
                        update.FileNameNew = model.Detail.FileNameNew;
                        update.FileNameType = model.Detail.FileNameType;
                        update.UpdatedAccount = sm.UserInfo.UserNo;
                        update.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C501M", dao.QueryForObject<Hashtable>("AM.LogGet__ReportRecord", where), where, "[ReportRecord]");
                        int rtn = dao.Update(update, where);
                        FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C501M", update, where, "[ReportRecord]");
                    }
                }
                #endregion                
                sm.LastResultMessage = "資料已更新";
            }
            else
            {
                return View("Detail", model);
            }
            return Index(model.Form);
        }

        [HttpPost]
        public ActionResult SendToAudit(C501MViewModel model)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            ReportRecord where = new ReportRecord();
            where.Seq = model.Detail.Seq;
            ReportRecord update = new ReportRecord();
            update.AuditStatus = "S";  // S：送件審核中
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C501M", dao.QueryForObject<Hashtable>("AM.LogGet__ReportRecord", where), where, "[ReportRecord]");
            int rtn = dao.Update(update, where);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C501M", update, where, "[ReportRecord]");
            sm.LastResultMessage = "已送出申請";
            return Index(model.Form);

        }

        /// <summary>
        /// 實際檔案上傳程序
        /// </summary>
        /// <param name="file"></param>
        /// <param name="model"></param>
        private void DoFileUpload(HttpPostedFileBase file, ref C501MViewModel model)
        {
            string fullPath = Server.MapPath(new AMDAO().UploadPathTeacherReport);
            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);  // 不存在則建立
            Random tmpNum = new Random();
            string tmpFileName =
                DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                Path.GetExtension(file.FileName);                   // 副檔名
            file.SaveAs(fullPath + "/" + tmpFileName);
            FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C501M", file.FileName, tmpFileName);
            // 變更 model 欄位 - 檔名
            model.Detail.FileNameNew = tmpFileName;
            model.Detail.FileNameOrg = file.FileName;
            model.Detail.FileNameType = Path.GetExtension(file.FileName);
        }

        /// <summary>
        /// 及時抓取 上傳繳交期限
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public ActionResult GetUploadDeadlineStr(string Year)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            string Result = dao.GetUploadDeadlineStr(Year, sm.UserInfo.User.UNITID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上傳前檢查 是否有重複<br />
        /// 空白 = 無
        /// </summary>
        /// <param name="Seq"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfirmUploadSame(string Year, string TeacherAccount, string ReportType)
        {
            A2DAO dao = new A2DAO();
            string ResultMsg = "";
            string[] readonlyArr = { "R", " " };  // (R：退件修正)、(半形空格(未送出))
            var getDBData = dao.GetRow(new ReportRecord()
            {
                Year = Year,
                TeacherAccount = TeacherAccount,
                ReportType = ReportType
            });
            if (getDBData != null && readonlyArr.Contains(getDBData.AuditStatus.TONotNullString()))
            {
                // 這邊只判斷  有資料 + 可覆蓋時的情況 去做判斷與訊息提示
                // 其餘狀態 交由 Save 去做儲存前檢查
                ResultMsg = "您已上傳過報告，是否確定要更新檔案資訊？";
            }
            return Json(ResultMsg, JsonRequestBehavior.AllowGet);
        }
    }
}