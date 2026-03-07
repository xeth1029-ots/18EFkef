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
    public class C502MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C502MViewModel model = new C502MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C502MFormModel form)
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
                form.Grid = dao.QueryC502M(form);
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
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            C502MViewModel model = new C502MViewModel();
            model.Detail = new C502MDetailModel();
            model.Detail.UnitCode = sm.UserInfo.User.UNITID;
            model.Detail.TeacherAccount = sm.UserInfo.UserNo;
            model.Detail.UploadDeadline = "請選擇";
            model.Detail.StudNum = 0;
            model.Detail.IsNew = true;
            return View("Detail", model);
        }

        [HttpGet]
        public ActionResult Detail(string Seq)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            // 檢查 Seq
            long iCheckSeq = -1;
            if (!long.TryParse(Seq, out iCheckSeq))
            {
                sm.LastErrorMessage = "非法操作！";
                return View("Index", new C502MFormModel());
            }
            A2DAO dao = new A2DAO();
            C502MViewModel model = new C502MViewModel();
            model.Detail = new C502MDetailModel();
            var tmpModel = dao.GetRow(new SelfReport() { Seq = iCheckSeq });
            if (tmpModel == null) return View("Index", new C502MFormModel());
            model.Detail.InjectFrom(tmpModel);
            model.Detail.UploadDeadline = dao.GetUploadDeadlineStr(model.Detail.Year, sm.UserInfo.User.UNITID);
            model.Detail.IsNew = false;
            return View("Detail", model);
        }

        private void FormatDataModel(ref C502MViewModel model)
        {
            // 移除非 "職能類別" 下的 "課程單元"
            string tmpJAC = model.Detail.JobAbilityCode.TONotNullString();
            if (tmpJAC != "")
            {
                var tmpList = model.Detail.CourseUnitCode.TONotNullString().Split(',').ToList();
                tmpList = tmpList.Where(x => x.ToLeft(2) == tmpJAC).ToList();
                model.Detail.CourseUnitCode = string.Join(",", tmpList);
                if (model.Detail.CourseUnitCode == "") model.Detail.CourseUnitCode = null;
            }
        }

        private string CheckDataModel(C502MViewModel model)
        {
            string Result = "";
            // 學員人數 檢查數值
            string tmpStr = model.Detail.StudNum.TONotNullString();
            int tmpInt = 0;
            if (!int.TryParse(tmpStr, out tmpInt))
            {
                Result = Result + "學員人數 需填寫數字<br/>";
            }
            // 檢查檔案大小是否 <= 20MB
            const int FileLimitSize = (20 * 1024 * 1024);
            if (model.Detail.FILE_Tslides != null && model.Detail.FILE_Tslides.ContentLength > FileLimitSize)
            {
                Result = Result + "教材簡報 檔案大小以 20MB 為限！<br/>";
            }
            if (model.Detail.FILE_StudyGReport != null && model.Detail.FILE_StudyGReport.ContentLength > FileLimitSize)
            {
                Result = Result + "學習分組報告 檔案大小以 20MB 為限！<br/>";
            }
            if (model.Detail.FILE_Worksheets != null && model.Detail.FILE_Worksheets.ContentLength > FileLimitSize)
            {
                Result = Result + "學習單 檔案大小以 20MB 為限！<br/>";
            }
            if (model.Detail.FILE_HomeWork != null && model.Detail.FILE_HomeWork.ContentLength > FileLimitSize)
            {
                Result = Result + "作業 檔案大小以 20MB 為限！<br/>";
            }
            if (model.Detail.FILE_ExpReport != null && model.Detail.FILE_ExpReport.ContentLength > FileLimitSize)
            {
                Result = Result + "心得報告 檔案大小以 20MB 為限！<br/>";
            }
            if (model.Detail.FILE_Exam != null && model.Detail.FILE_Exam.ContentLength > FileLimitSize)
            {
                Result = Result + "測驗 檔案大小以 20MB 為限！<br/>";
            }
            if (model.Detail.FILE_Other1 != null && model.Detail.FILE_Other1.ContentLength > FileLimitSize)
            {
                Result = Result + "其他一附檔 檔案大小以 20MB 為限！<br/>";
            }
            if (model.Detail.FILE_Other2 != null && model.Detail.FILE_Other2.ContentLength > FileLimitSize)
            {
                Result = Result + "其他一附檔 檔案大小以 20MB 為限！<br/>";
            }

            if (model.Detail.FILE_Tslides != null && !FileTypeDetector.IsFileTypeValid(model.Detail.FILE_Tslides, model.Detail.FILE_Tslides.FileName))
            {
                Result = string.Concat(Result, "教材簡報 檔案內容有誤 無效的檔案格式(限.doc, .docx, .ppt, .pdf, .odt, .odp檔案)！<br/>");
            }
            if (model.Detail.FILE_StudyGReport != null && !FileTypeDetector.IsFileTypeValidpdf(model.Detail.FILE_StudyGReport))
            {
                Result = Result + "學習分組報告 檔案內容有誤 無效的檔案格式(限pdf檔案)！<br/>";
            }
            if (model.Detail.FILE_Worksheets != null && !FileTypeDetector.IsFileTypeValidpdf(model.Detail.FILE_Worksheets))
            {
                Result = Result + "學習單 檔案內容有誤 無效的檔案格式(限pdf檔案)！<br/>";
            }
            if (model.Detail.FILE_HomeWork != null && !FileTypeDetector.IsFileTypeValidpdf(model.Detail.FILE_HomeWork))
            {
                Result = Result + "作業 檔案內容有誤 無效的檔案格式(限pdf檔案)！<br/>";
            }
            if (model.Detail.FILE_ExpReport != null && !FileTypeDetector.IsFileTypeValidpdf(model.Detail.FILE_ExpReport))
            {
                Result = Result + "心得報告 檔案內容有誤 無效的檔案格式(限pdf檔案)！<br/>";
            }
            if (model.Detail.FILE_Exam != null && !FileTypeDetector.IsFileTypeValidpdf(model.Detail.FILE_Exam))
            {
                Result = Result + "測驗 檔案內容有誤 無效的檔案格式(限pdf檔案)！<br/>";
            }
            if (model.Detail.FILE_Other1 != null && !FileTypeDetector.IsFileTypeValidpdf(model.Detail.FILE_Other1))
            {
                Result = Result + "其他一附檔 檔案內容有誤 無效的檔案格式(限pdf檔案)！<br/>";
            }
            if (model.Detail.FILE_Other2 != null && !FileTypeDetector.IsFileTypeValidpdf(model.Detail.FILE_Other2))
            {
                Result = Result + "其他一附檔 檔案內容有誤 無效的檔案格式(限pdf檔案)！<br/>";
            }
            return Result;
        }

        private string CheckUploadRange(C502MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            if (model.Detail.Year.TONotNullString() == "") return "";
            string Msg = "";
            A2DAO dao = new A2DAO();
            var tmpObj = dao.GetRowList(new Report() { Year = model.Detail.Year, UnitCode = sm.UserInfo.User.UNITID }).FirstOrDefault();
            if (tmpObj != null)
            {
                var DateS = DateTime.Parse(tmpObj.DateS.TONotNullString());
                var DateE = DateTime.Parse(tmpObj.DateE.TONotNullString());
                if (DateTime.Now < DateS || DateTime.Now > DateE) Msg = "已超過繳交期限，無法送件！<br/>";
            }
            return Msg;
        }

        private string CheckUploadFiles(C502MViewModel model)
        {
            // 由程式檢查上傳附檔是否都有填
            // 因如果上傳欄位物件 = null 並不一定代表 之前沒有上傳過
            // 應該兩個都檢查 (上傳欄位的物件) + (上傳欄位Org, 上傳欄位New, 上傳欄位Type)
            string Msg = "";
            // 教材簡報
            if (model.Detail.FILE_Tslides == null
             && model.Detail.TslidesOrg.TONotNullString() == ""
             && model.Detail.TslidesNew.TONotNullString() == ""
             && model.Detail.TslidesType.TONotNullString() == "") { Msg = Msg + "教材簡報 欄位是必要項。<br/><br/>"; }
            //// 學習分組報告
            //if (model.Detail.FILE_StudyGReport == null
            // && model.Detail.StudyGReportOrg.TONotNullString() == ""
            // && model.Detail.StudyGReportNew.TONotNullString() == ""
            // && model.Detail.StudyGReportType.TONotNullString() == "") { Msg = Msg + "學習分組報告 欄位是必要項。<br/><br/>"; }
            //// 學習單
            //if (model.Detail.FILE_Worksheets == null
            // && model.Detail.WorksheetsOrg.TONotNullString() == ""
            // && model.Detail.WorksheetsNew.TONotNullString() == ""
            // && model.Detail.WorksheetsType.TONotNullString() == "") { Msg = Msg + "學習單 欄位是必要項。<br/><br/>"; }
            //// 作業
            //if (model.Detail.FILE_HomeWork == null
            // && model.Detail.HomeWorkOrg.TONotNullString() == ""
            // && model.Detail.HomeWorkNew.TONotNullString() == ""
            // && model.Detail.HomeWorkType.TONotNullString() == "") { Msg = Msg + "作業 欄位是必要項。<br/><br/>"; }
            //// 心得報告
            //if (model.Detail.FILE_ExpReport == null
            // && model.Detail.ExpReportOrg.TONotNullString() == ""
            // && model.Detail.ExpReportNew.TONotNullString() == ""
            // && model.Detail.ExpReportType.TONotNullString() == "") { Msg = Msg + "心得報告 欄位是必要項。<br/><br/>"; }
            //// 測驗
            //if (model.Detail.FILE_Exam == null
            // && model.Detail.ExamOrg.TONotNullString() == ""
            // && model.Detail.ExamNew.TONotNullString() == ""
            // && model.Detail.ExamType.TONotNullString() == "") { Msg = Msg + "測驗 欄位是必要項。<br/><br/>"; }
            // 改成：5項至少需填一項
            int FillCount = 0;
            // 學習分組報告
            if (model.Detail.FILE_StudyGReport == null
             && model.Detail.StudyGReportOrg.TONotNullString() == ""
             && model.Detail.StudyGReportNew.TONotNullString() == ""
             && model.Detail.StudyGReportType.TONotNullString() == "") FillCount++;
            // 學習單
            if (model.Detail.FILE_Worksheets == null
             && model.Detail.WorksheetsOrg.TONotNullString() == ""
             && model.Detail.WorksheetsNew.TONotNullString() == ""
             && model.Detail.WorksheetsType.TONotNullString() == "") FillCount++;
            // 作業
            if (model.Detail.FILE_HomeWork == null
             && model.Detail.HomeWorkOrg.TONotNullString() == ""
             && model.Detail.HomeWorkNew.TONotNullString() == ""
             && model.Detail.HomeWorkType.TONotNullString() == "") FillCount++;
            // 心得報告
            if (model.Detail.FILE_ExpReport == null
             && model.Detail.ExpReportOrg.TONotNullString() == ""
             && model.Detail.ExpReportNew.TONotNullString() == ""
             && model.Detail.ExpReportType.TONotNullString() == "") FillCount++;
            // 測驗
            if (model.Detail.FILE_Exam == null
             && model.Detail.ExamOrg.TONotNullString() == ""
             && model.Detail.ExamNew.TONotNullString() == ""
             && model.Detail.ExamType.TONotNullString() == "") FillCount++;
            if (FillCount == 5) { Msg = Msg + "「教學/訓練成效成果證據」，請至少選擇其中一項上傳。<br/><br/>"; }
            return Msg;
        }

        private string GenerateFileName(string fileName)
        {
            Random tmpNum = new Random();
            return
                DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                Path.GetExtension(fileName);                        // 副檔名
        }

        /// <summary>執行附檔上傳儲存程序</summary>
        private void SaveFilesProcess(ref C502MViewModel model)
        {
            string fullPath = Server.MapPath(new AMDAO().UploadPathTeacherReport);
            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);  // 不存在則建立
            #region 教材簡報
            if (model.Detail.FILE_Tslides != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_Tslides.FileName);
                model.Detail.FILE_Tslides.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_Tslides.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.TslidesNew = tmpFileName;
                model.Detail.TslidesOrg = model.Detail.FILE_Tslides.FileName;
                model.Detail.TslidesType = Path.GetExtension(model.Detail.FILE_Tslides.FileName);
            }
            #endregion
            #region 學習分組報告
            if (model.Detail.FILE_StudyGReport != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_StudyGReport.FileName);
                model.Detail.FILE_StudyGReport.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_StudyGReport.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.StudyGReportNew = tmpFileName;
                model.Detail.StudyGReportOrg = model.Detail.FILE_StudyGReport.FileName;
                model.Detail.StudyGReportType = Path.GetExtension(model.Detail.FILE_StudyGReport.FileName);
            }
            #endregion
            #region 學習單
            if (model.Detail.FILE_Worksheets != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_Worksheets.FileName);
                model.Detail.FILE_Worksheets.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_Worksheets.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.WorksheetsNew = tmpFileName;
                model.Detail.WorksheetsOrg = model.Detail.FILE_Worksheets.FileName;
                model.Detail.WorksheetsType = Path.GetExtension(model.Detail.FILE_Worksheets.FileName);
            }
            #endregion
            #region 作業
            if (model.Detail.FILE_HomeWork != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_HomeWork.FileName);
                model.Detail.FILE_HomeWork.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_HomeWork.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.HomeWorkNew = tmpFileName;
                model.Detail.HomeWorkOrg = model.Detail.FILE_HomeWork.FileName;
                model.Detail.HomeWorkType = Path.GetExtension(model.Detail.FILE_HomeWork.FileName);
            }
            #endregion
            #region 心得報告
            if (model.Detail.FILE_ExpReport != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_ExpReport.FileName);
                model.Detail.FILE_ExpReport.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_ExpReport.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.ExpReportNew = tmpFileName;
                model.Detail.ExpReportOrg = model.Detail.FILE_ExpReport.FileName;
                model.Detail.ExpReportType = Path.GetExtension(model.Detail.FILE_ExpReport.FileName);
            }
            #endregion
            #region 測驗
            if (model.Detail.FILE_Exam != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_Exam.FileName);
                model.Detail.FILE_Exam.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_Exam.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.ExamNew = tmpFileName;
                model.Detail.ExamOrg = model.Detail.FILE_Exam.FileName;
                model.Detail.ExamType = Path.GetExtension(model.Detail.FILE_Exam.FileName);
            }
            #endregion
            #region 其他一
            if (model.Detail.FILE_Other1 != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_Other1.FileName);
                model.Detail.FILE_Other1.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_Other1.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.Other1New = tmpFileName;
                model.Detail.Other1Org = model.Detail.FILE_Other1.FileName;
                model.Detail.Other1Type = Path.GetExtension(model.Detail.FILE_Other1.FileName);
            }
            #endregion
            #region 其他二
            if (model.Detail.FILE_Other2 != null)
            {
                // 存檔
                string tmpFileName = GenerateFileName(model.Detail.FILE_Other2.FileName);
                model.Detail.FILE_Other2.SaveAs(fullPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C502M", model.Detail.FILE_Other2.FileName, tmpFileName);
                // 變更 model 欄位
                model.Detail.Other2New = tmpFileName;
                model.Detail.Other2Org = model.Detail.FILE_Other2.FileName;
                model.Detail.Other2Type = Path.GetExtension(model.Detail.FILE_Other2.FileName);
            }
            #endregion
        }

        /// <summary>共用儲存程序 (暫存、正式儲存、審核) 都會走這段</summary>        
        /// <returns>true: 儲存成功 <br/> false: 儲存失敗</returns>
        private bool SaveCommonProcess(ref SessionModel sm, ref C502MViewModel model)
        {
            bool Result = true;
            A2DAO dao = new A2DAO();
            model.Detail.UnitCode = sm.UserInfo.User.UNITID;
            model.Detail.TeacherAccount = sm.UserInfo.UserNo;
            model.Detail.UpdatedAccount = sm.UserInfo.UserNo;
            model.Detail.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            // 檢查是否已有資料，有則更新舊資料
            var getDBData = dao.GetRow(new SelfReport()
            {
                Year = model.Detail.Year,
                TeacherAccount = model.Detail.TeacherAccount,
            });
            if (getDBData != null || model.Detail.Seq != null || model.Detail.Seq > 0)
            {
                // 是(已上傳過) ->
                //   (1) 若【審核狀態】已為 (Y：審核通過)、(N：審核不通過)、(S：送件審核中)，不可更新。
                //       -> 跳出提示："此份報告已送審，不可再更新。"、"此份報告已審核，不可再更新。"
                //   (2) 若【審核狀態】已為 (R：退件修正)、(半形空格(未送出))，可更新，
                //       -> 跳出提示："您已上傳過報告，是否確定要更新檔案資訊?"若是，update資料，但不可蓋過【首次上傳時間】。
                string[] readonlyArr = { "S", "Y", "N" };
                if (readonlyArr.Contains(getDBData.AuditStatus.TONotNullString()))
                {
                    if (getDBData.AuditStatus == "S") sm.LastErrorMessage = "此份報告已送審，不可再更新！";
                    else sm.LastErrorMessage = "此份報告已審核，不可再更新！";
                    Result = false;
                }
                else
                {
                    SelfReport where = new SelfReport();
                    where.Seq = getDBData.Seq;
                    SelfReport update = new SelfReport();
                    this.SaveFilesProcess(ref model);  // 執行附檔上傳儲存程序
                    update.InjectFrom(model.Detail);
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C502M", dao.QueryForObject<Hashtable>("AM.LogGet__SelfReport", where), where, "[SelfReport]: TempSave");
                    int res = dao.Update(update, where);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C502M", update, where, "[SelfReport]: TempSave");
                    model.Detail.Seq = getDBData.Seq;  // 回填唯一值
                }
            }
            else
            {
                // 否 -> 新增一筆資料
                SelfReport InsObj = new SelfReport();
                this.SaveFilesProcess(ref model);  // 執行附檔上傳儲存程序
                InsObj.InjectFrom(model.Detail);
                int res = dao.Insert(InsObj);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, ModifyState.SUCCESS, "A2/C502M", InsObj, null, "[SelfReport]: TempSave");
                model.Detail.Seq = res;  // 回填唯一值
            }
            return Result;
        }

        /// <summary>暫存程序</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TempSave(C502MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            string Msg = "";
            // 檢查一下資料 (非必填檢查)
            Msg = this.CheckDataModel(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 檢查一下繳交期限
            Msg = this.CheckUploadRange(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 整理一下資料
            this.FormatDataModel(ref model);

            // 儲存程序開始
            if (model.Detail.Year.TONotNullString() != "" && model.Detail.JobAbilityCode.TONotNullString() != "")
            {
                ModelState.Clear();
                if (!this.SaveCommonProcess(ref sm, ref model)) return View("Detail", model);
                sm.LastResultMessage = "已暫存";
            }
            else
            {
                sm.LastErrorMessage = "暫存時，請至少要選擇年度、職能類別欄位！";
                return View("Detail", model);
            }
            return Index(model.Form);
        }

        /// <summary>儲存程序</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(C502MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            string Msg = "";
            // 檢查一下資料 (非必填檢查)
            Msg = this.CheckDataModel(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 檢查一下繳交期限
            Msg = this.CheckUploadRange(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 檢查一下檔案 (必填檢查)
            Msg = this.CheckUploadFiles(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 整理一下資料
            this.FormatDataModel(ref model);

            // 儲存程序開始
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                if (!this.SaveCommonProcess(ref sm, ref model)) return View("Detail", model);
                sm.LastResultMessage = "已儲存";
            }
            else
            {
                return View("Detail", model);
            }
            return Index(model.Form);
        }

        [HttpPost]
        public ActionResult SendToAudit(C502MViewModel model)
        {
            // 注意！審核程序前段部，都跟正式儲存一樣，只是後段部多一個改狀態的動作而已
            // 因為怕使用者只有按暫存，而沒有按正式儲存，就直接按審核按鈕
            // 所以這邊直接當作幫他按正式儲存
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            string Msg = "";
            // 檢查一下資料 (非必填檢查)
            Msg = this.CheckDataModel(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 檢查一下繳交期限
            Msg = this.CheckUploadRange(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 檢查一下檔案 (必填檢查)
            Msg = this.CheckUploadFiles(model); if (Msg != "") { sm.LastErrorMessage = Msg; return View("Detail", model); }
            // 整理一下資料
            this.FormatDataModel(ref model);

            // 儲存程序開始
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                if (!this.SaveCommonProcess(ref sm, ref model)) return View("Detail", model);
                // 開始審核
                if (model.Detail.Seq.HasValue && model.Detail.Seq.Value > 0)
                {
                    // 因為前面存過了，所以這邊直接拿 Seq 來當條件存 (裡面有回寫唯一值 key)
                    var where = new SelfReport() { Seq = model.Detail.Seq };
                    // S：送件審核中
                    var update = new SelfReport() { AuditStatus = "S" };  
                    // 如果是第一次，則存入【首次上傳時間】
                    if (model.Detail.AuditStatus.TONotNullString() == "") update.FirstTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    // GO
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C502M", dao.QueryForObject<Hashtable>("AM.LogGet__SelfReport", where), where, "[SelfReport]: SendToAudit");
                    int res = dao.Update(update, where);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C502M", update, where, "[SelfReport]: SendToAudit");
                }
                sm.LastResultMessage = "已送出申請";
            }
            else
            {
                return View("Detail", model);
            }
            return Index(model.Form);
        }

        [HttpPost]
        public ActionResult Delete(C502MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            // 檢查 Seq
            long iCheckSeq = -1;
            if (!long.TryParse(form.Seq_forDel, out iCheckSeq))
            {
                sm.LastErrorMessage = "非法操作！";
                return Index(form);
            }
            // 刪除程序開始
            A2DAO dao = new A2DAO();
            var tmpObj = dao.GetRow(new SelfReport() { Seq = iCheckSeq });
            if (tmpObj != null)
            {
                // 刪除檔案
                string fullPath = Server.MapPath(new AMDAO().UploadPathTeacherReport) + "/";
                string[] TheFiles = {
                    fullPath + tmpObj.TslidesNew.TONotNullString(),
                    fullPath + tmpObj.StudyGReportNew.TONotNullString(),
                    fullPath + tmpObj.WorksheetsNew.TONotNullString(),
                    fullPath + tmpObj.HomeWorkNew.TONotNullString(),
                    fullPath + tmpObj.ExpReportNew.TONotNullString(),
                    fullPath + tmpObj.ExamNew.TONotNullString(),
                    fullPath + tmpObj.Other1New.TONotNullString(),
                    fullPath + tmpObj.Other2New.TONotNullString(),
                };
                foreach (var file in TheFiles) if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                // 刪除資料
                var where = new SelfReport() { Seq = iCheckSeq };
                FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A2/C502M", dao.QueryForListAll<Hashtable>("AM.LogGet__SelfReport", where), where, "[SelfReport]");
                var res = dao.Delete(where);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (res == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A2/C502M", null, where, "[SelfReport]");
                sm.LastResultMessage = "已刪除";
            }
            return Index(form);
        }

        /// <summary>及時抓取 上傳繳交期限</summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public ActionResult GetUploadDeadlineStr(string Year)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            string Result = dao.GetUploadDeadlineStr(Year, sm.UserInfo.User.UNITID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>上傳前檢查 是否有重複<br />空白 = 無</summary>
        /// <param name="Seq"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfirmUploadSame(string Year, string TeacherAccount)
        {
            if (Year.TONotNullString() == "") Year = "-1";
            A2DAO dao = new A2DAO();
            string ResultMsg = "";
            string[] readonlyArr = { "R", " ", "" };  // (R：退件修正)、(空白：未送出)
            var getDBData = dao.GetRow(new SelfReport()
            {
                Year = Year,
                TeacherAccount = TeacherAccount,
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