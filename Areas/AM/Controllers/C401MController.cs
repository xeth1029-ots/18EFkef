using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.AM.Models;
using WKEFSERVICE.Models;
using System.Linq;
using WKEFSERVICE.Services;
using Turbo.Commons;
using OfficeOpenXml;
using System.Drawing;
using System;
using System.IO;
using System.Threading;
using System.Web;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C401MController : WKEFSERVICE.Controllers.BaseController
    {
        /// <summary> 教師資料表欄位名稱<br /> 對應查詢條件的順序 Index </summary>
        private readonly string[] ExportTableFields = {
            "ACCOUNT",
            //"IDNO",  // 稽核 隱藏
            "TeacherEName",
            "Sex_Name",
            "Birthday",
            "Tel",
            "Phone",
            "Email",
            "EmailWork",
            "Address",
            "EduLevelHighest_Name",
            "EduSchool1,EduDept1,EduLevel1,EduSchool2,EduDept2,EduLevel2,EduSchool3,EduDept3,EduLevel3",
            "ServiceUnit1,JobTitle1,ServiceUnit2,JobTitle2",
            "ExpertiseDesc",
            "Expertise_Str",
            "TeachJobAbilityDC,TeachJobAbilityBC,TeachJobAbilityKC",
            "TeachArea",
            "IndustryStr",
            "IndustryAcademicType_Name",
            "WorkHistory",
            "ProLicense",
            "SelfIntroduction",
            "JoinYear",
            "ServiceUnitProperties_Name",
            "PublicCore_Name",
            "Online_Name",
            "OfflineDate",
            "OfflineReason_Str",
            "OfflineReasonRemark",
            "HomePageCarousel_Name"
        };

        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C401MViewModel model = new C401MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C401MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ActionResult rtn = View("Index", form);
            AMDAO dao = new AMDAO();
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 查詢
                form.Grid = dao.QueryC401M(form);
                if (form.Grid.ToCount() <= 0) { sm.LastResultMessage = "查無資料"; }
                else
                {
                    // 建立 Excel 物件
                    ExcelPackage objExcel = new ExcelPackage();
                    ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add("教師資料明細");
                    // 一些暫存變數
                    int Row = 1;
                    int Col = 1;
                    int ColMax = 2;
                    // 產生 標題列 (固定欄位)
                    objSheet.Cells[Row, Col].Value = "所屬轄區"; Col++;
                    objSheet.Cells[Row, Col].Value = "姓名(中文)"; Col++;
                    // 產生 標題列 (動態欄位)
                    if (form.ExportFields.TONotNullString() != "")
                    {
                        foreach (var dataCol in form.ExportFields.TONotNullString().Split(','))
                        {
                            int idx = -1;
                            if (!int.TryParse(dataCol, out idx)) continue;
                            string fieldName = ExportTableFields[idx];
                            if (fieldName == "EduSchool1,EduDept1,EduLevel1,EduSchool2,EduDept2,EduLevel2,EduSchool3,EduDept3,EduLevel3" ||
                                fieldName == "ServiceUnit1,JobTitle1,ServiceUnit2,JobTitle2")
                            {
                                // 複合欄位特別處理
                                string tmpStr = fieldName
                                    .Replace("EduSchool", "學歷背景-學校").Replace("EduDept", "學歷背景-系別").Replace("EduLevel", "學歷背景-學位")
                                    .Replace("ServiceUnit", "服務單位").Replace("JobTitle", "職稱");
                                foreach (var tmpStrItm in tmpStr.Split(','))
                                {
                                    objSheet.Cells[Row, Col].Value = tmpStrItm;
                                    Col++;
                                }
                            }
                            else
                            {
                                objSheet.Cells[Row, Col].Value = form.ExportFields_SHOW_list.Where(x => x.Value == dataCol).FirstOrDefault().DisplayText;
                                Col++;
                            }
                            ColMax = Col - 1;
                        }
                    }
                    Row = 2;
                    Col = 1;
                    // 寫入 資料列
                    foreach (var dataRow in form.Grid)
                    {
                        // 固定欄位
                        objSheet.Cells[Row, Col].Value = dataRow.UnitName; Col++;
                        objSheet.Cells[Row, Col].Value = dataRow.TeacherName; Col++;
                        // 動態欄位
                        if (form.ExportFields.TONotNullString() != "")
                        {
                            foreach (var dataCol in form.ExportFields.TONotNullString().Split(','))
                            {
                                string dataValue = "";
                                int idx = -1;
                                if (!int.TryParse(dataCol, out idx)) continue;
                                string fieldName = ExportTableFields[idx];
                                if (fieldName.Contains(","))
                                {
                                    // 複合欄位特別處理
                                    if (fieldName == "EduSchool1,EduDept1,EduLevel1,EduSchool2,EduDept2,EduLevel2,EduSchool3,EduDept3,EduLevel3")
                                    {
                                        bool isEdu1 = (dataRow.EduSchool1.TONotNullString() != "" && dataRow.EduDept1.TONotNullString() != "");
                                        bool isEdu2 = (dataRow.EduSchool2.TONotNullString() != "" && dataRow.EduDept2.TONotNullString() != "");
                                        bool isEdu3 = (dataRow.EduSchool3.TONotNullString() != "" && dataRow.EduDept3.TONotNullString() != "");
                                        objSheet.Cells[Row, Col].Value = dataRow.EduSchool1; Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.EduDept1; Col++;
                                        objSheet.Cells[Row, Col].Value = (isEdu1 ? dataRow.EduLevel1_Name : ""); Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.EduSchool2; Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.EduDept2; Col++;
                                        objSheet.Cells[Row, Col].Value = (isEdu2 ? dataRow.EduLevel2_Name : ""); Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.EduSchool3; Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.EduDept3; Col++;
                                        objSheet.Cells[Row, Col].Value = (isEdu3 ? dataRow.EduLevel3_Name : ""); Col++;
                                        continue;
                                        //dataValue =
                                        //    dataRow.EduSchool1 + " " + dataRow.EduDept1 + " " + (isEdu1 ? dataRow.EduLevel1_Name : "") + "\r\n" +
                                        //    dataRow.EduSchool2 + " " + dataRow.EduDept2 + " " + (isEdu2 ? dataRow.EduLevel2_Name : "") + "\r\n" +
                                        //    dataRow.EduSchool3 + " " + dataRow.EduDept3 + " " + (isEdu3 ? dataRow.EduLevel3_Name : "");
                                        //dataValue = dataValue.Trim();
                                    }
                                    else if (fieldName == "ServiceUnit1,JobTitle1,ServiceUnit2,JobTitle2")
                                    {
                                        objSheet.Cells[Row, Col].Value = dataRow.ServiceUnit1; Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.JobTitle1; Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.ServiceUnit2; Col++;
                                        objSheet.Cells[Row, Col].Value = dataRow.JobTitle2; Col++;
                                        continue;
                                        //dataValue =
                                        //    dataRow.ServiceUnit1 + " " + dataRow.JobTitle1 + "\r\n" +
                                        //    dataRow.ServiceUnit2 + " " + dataRow.JobTitle2;
                                        //dataValue = dataValue.Trim();
                                    }
                                    else if (fieldName == "TeachJobAbilityDC,TeachJobAbilityBC,TeachJobAbilityKC")
                                    {
                                        if (dataRow.TeachJobAbilityDC == "Y") dataValue = dataValue + "DC,";
                                        if (dataRow.TeachJobAbilityBC == "Y") dataValue = dataValue + "BC,";
                                        if (dataRow.TeachJobAbilityKC == "Y") dataValue = dataValue + "KC,";
                                        if (dataValue.ToRight(1) == ",") dataValue = dataValue.Substring(0, dataValue.Length - 1);
                                    }
                                }
                                else
                                {
                                    // 單欄直接抓取
                                    dataValue = dataRow.GetType().GetProperty(fieldName).GetValue(dataRow, null).TONotNullString();
                                }
                                objSheet.Cells[Row, Col].Value = dataValue;
                                Col++;
                            }
                        }
                        Row++;
                        Col = 1;
                    }
                    // 後續處理
                    objSheet.Cells.AutoFitColumns(10, 35);
                    objSheet.Cells.Style.Font.Name = "新細明體";
                    objSheet.Cells.Style.Font.Size = 12;
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255, 204));
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Left.Color.SetColor(Color.FromArgb(255, 178, 178, 178));
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Right.Color.SetColor(Color.FromArgb(255, 178, 178, 178));
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Top.Color.SetColor(Color.FromArgb(255, 178, 178, 178));
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.SelectedRange[1, 1, 1, ColMax].Style.Border.Bottom.Color.SetColor(Color.FromArgb(255, 178, 178, 178));

                    string s_FileName1 = string.Concat("匯出教師資料明細", "_", CommonsServices.GetDateTimeNow1());
                    string s_FileName1_xlsx = string.Concat(s_FileName1, ".xlsx");
                    //sm.LastResultMessage = "已匯出";
                    FuncLogAdd.Do(ModifyEvent.NONE, ModifyType.PRINT, ModifyState.SUCCESS, "AM/C401M", null, form, s_FileName1 + (form.ExportFormat == "0" ? ".xlsx" : ".ods"));
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
            }
            return rtn;
        }
    }
}