using OfficeOpenXml;
using System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Turbo.Commons;
using WKEFSERVICE.Commons;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using WKEFSERVICE.Areas.AM.Models;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C404MController : WKEFSERVICE.Controllers.BaseController
    {
        //管理者首頁-資料匯出-年度評核結果總表-資料匯出-年度評核結果總表 GET: AM/C404M
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C404MViewModel model = new C404MViewModel();
            var aNow = DateTime.Now;
            model.Form.Year = ((aNow.Month < 6) ? aNow.Year - 1 : aNow.Year).ToString();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C404MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear(); //ModelState.Clear();
            ShareCodeListModel sclm = new ShareCodeListModel();
            ActionResult rtn = View("Index", form);
            if (string.IsNullOrEmpty(form.Year) || $"{form.Year}"== "") { sm.LastErrorMessage = "請選擇計畫年度！"; return rtn; }
            AMDAO dao = new AMDAO();

            // 暫存資料 老師資料表 //年度評核結果總表 // Online = "Y", 
            //form.TeacherUnit = sm.UserInfo.User.UNITID;
            Teacher whereTeacher = new Teacher();
            if (!string.IsNullOrEmpty(form.TeacherUnit) && form.TeacherUnit != "0")
            {
                whereTeacher = new Teacher() { UnitCode = form.TeacherUnit };
            }
            List<Teacher> lstTeachers = dao.GetRowList(whereTeacher).ToList();
            //教師姓名：文字框，輸入關鍵字，針對姓名欄位做模糊查詢
            if (lstTeachers != null && !string.IsNullOrEmpty(form.TeacherName) && form.TeacherName.Length > 0)
            {
                lstTeachers = lstTeachers.Where(x => x.TeacherName.IndexOf(form.TeacherName) > -1).ToList();
            }
            if (lstTeachers == null || lstTeachers.ToCount() == 0) { sm.LastErrorMessage = "查無資料！"; return rtn; }
            lstTeachers = lstTeachers.OrderBy(x => x.UnitCode).ThenBy(x => x.TeacherName).ToList();

            //var lstReviewReport = dao.GetRowList(new ReviewReport() { UnitCode = form.TeacherUnit }).ToList();
            var whereReview = new ReviewReport() { Year = form.Year };
            List<ReviewReport> lstReviewReport = dao.GetRowList(whereReview).ToList();
            //string s_TMP1 = "";
            try
            {
                //var YEARS_ROC = (form.Year.TOInt32() - 1911).ToString();
                var s_SheetNM = "年度評核結果總表";
                int iCol_MaxLength = 40;

                //建立 Excel 物件
                ExcelPackage objExcel = new ExcelPackage();
                ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add(s_SheetNM);

                //年度評核結果總表 匯出程序 TITLE
                //ShowExcelTitle(form, objSheet);
                ExcelTitle et = new ExcelTitle();
                Hashtable ht = new Hashtable { { "fmYear", form.Year } };
                et.ShowExcelTitle1(objSheet, ht);

                //資料面匯出
                Hashtable ht2 = new Hashtable { { "fmYear", form.Year }, { "Col_MaxLength", iCol_MaxLength } };
                et.SetExcelData(objSheet, ht2, lstTeachers, lstReviewReport);

                string s_FileName1 = string.Concat("年度評核結果總表", "_", CommonsServices.GetDateTimeNow1());
                string s_FileName1_xlsx = string.Concat(s_FileName1, ".xlsx");
                // OK
                FuncLogAdd.Do(ModifyEvent.NONE, ModifyType.PRINT, ModifyState.SUCCESS, "AM/C404M", null, form, string.Concat(s_FileName1, (form.ExportFormat == "0" ? ".xlsx" : ".ods")));
                if (form.ExportFormat == "0")
                {
                    return File(objExcel.GetAsByteArray(), HelperUtil.XLSXContentType, s_FileName1_xlsx);
                }
                else if (form.ExportFormat == "1")
                {
                    // 轉 ODS
                    ActionResult rtnOds = File(objExcel.GetAsByteArray(), HelperUtil.XLSXContentType, s_FileName1_xlsx);
                    HttpResponseBase response = ControllerContext.RequestContext.HttpContext.Response;
                    response.Filter = new Turbo.ReportTK.ExcelToOdsFilter(response);
                    return rtnOds;
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                sm.LastErrorMessage = "發生錯誤！";
            }
            return rtn;
        }
    }
}