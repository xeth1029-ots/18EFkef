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
using System.Collections;
using OfficeOpenXml;
using System.Drawing;

namespace WKEFSERVICE.Areas.A2.Controllers
{
    public class C202MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            C202MViewModel model = new C202MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C202MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            ActionResult rtn = View("Index", form);
            if (!ModelState.IsValid) { return rtn; }
            ModelState.Clear();
            // 設定查詢分頁資訊
            dao.SetPageInfo(form.rid, form.p);
            // 查詢結果
            form.LoginIDNO = sm.UserInfo.User.IDNO;

            form.Grid = dao.QueryC202M(form, form.PlanTypeCode, false);
            // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
            if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
            // 設定分頁元件(_PagingLink partial view)所需的資訊
            base.SetPagingParams(form, dao, "Index");

            return rtn;
        }

        [HttpPost]
        public ActionResult Export(C202MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            ActionResult rtn = View("Index", form);
            if (form.Year.TONotNullString() == "") { sm.LastErrorMessage = "請選擇計畫年度！"; return rtn; }

            A2DAO dao = new A2DAO();
            form.LoginIDNO = sm.UserInfo.User.IDNO;
            // A2/C202M - 教師首頁>教師資料>個人授課資料：匯出師資個人授課總表
            form.Grid = dao.QueryC202M(form, form.PlanTypeCode, true);
            if (form.Grid.ToCount() <= 0) { sm.LastErrorMessage = "查無資料！"; return rtn; }

            // 匯出程序
            try
            {
                // 建立 Excel 物件
                ExcelPackage objExcel = new ExcelPackage();
                ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add("師資個人授課總表");
                // 一些暫存變數
                string[] DisplayCols = {
                    "計畫別",
                    "授課內容所署分署",
                    //"申請單位",
                    "訓練單位",
                    //"計畫/訓練案號",
                    "學程/課程名稱",
                    "開訓日期","結訓日期",
                    "授課起日","授課迄日",
                    "授課時間起","授課時間迄",
                    "授課職能類別",
                    "授課單元",
                    "授課時數",
                    //"上課人數", //"問卷填答人數", 授課滿意度達70%以上對象表示滿意情形
                    "授課對象針對師資授課表示滿意之人數達70%以上",
                    "建議事項\r\n(意見或建議)"
                };
                string[] DisplayFields = {
                    "PlanTypeCode",
                    "UnitCode_Text",
                    //"ApplyUnit",
                    "TrainingUnit",
                    //"PlanID",
                    "ClassName",
                    "TrainingDateS","TrainingDateE",
                    "ClassDateS","ClassDateE",
                    "TrainingTimeS","TrainingTimeE",
                    "JobAbilityName",
                    "CourseUnitName",
                    "TeachHours",
                    //"TeachNum", //"FillStudentNum",
                    "Satisfy",
                    "Suggest"
                };
                // 產生 標題列
                objSheet.Cells[1, 1].Value = (form.Year.TOInt32() - 1911).ToString() + "年度關鍵就業力課程師資個人授課總表";
                objSheet.Cells[2, 1].Value = "講師姓名：";
                objSheet.Cells[2, 3].Value = "所屬轄區：";
                for (int i = 1; i <= DisplayCols.Length; i++)
                {
                    objSheet.Cells[4, i].Value = DisplayCols[i - 1];
                    objSheet.Cells[4, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[4, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[4, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[4, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                // 開始塞資料
                objSheet.Cells[2, 2].Value = sm.UserInfo.User.USERNAME;
                objSheet.Cells[2, 4].Value = sm.UserInfo.User.UNIT_NAME;

                string[] aPTCode23 = { "2", "3" };
                ShareCodeListModel sclm = new ShareCodeListModel();
                int Row = 5;
                foreach (var row in form.Grid)
                {
                    for (int i = 1; i <= DisplayCols.Length; i++)
                    {
                        // 特別處理欄位
                        if (DisplayFields[i - 1] == "PlanTypeCode")
                        {
                            objSheet.Cells[Row, i].Value = sclm.TransData_PlanType_List().Where(x => x.Value == row.PlanTypeCode.TONotNullString()).FirstOrDefault().Text;
                        }
                        else if (DisplayFields[i - 1] == "Satisfy")
                        {
                            objSheet.Cells[Row, i].Value = string.IsNullOrEmpty(row.Satisfy) ? "" : string.Concat(row.Satisfy, "%");
                        }
                        else if (DisplayFields[i - 1] == "Suggest")
                        {
                            if (row.PlanTypeCode == "1")
                            {
                                var tmpList = dao.GetRowList(new TransData_D_Satisfy()
                                {
                                    PlanID = row.PlanID,
                                    Year = row.Year,
                                    CourseUnitCode = row.CourseUnitCode,
                                    TeacherID = row.TeacherID,
                                });
                                if (tmpList.Any()) objSheet.Cells[Row, i].Value = string.Join("\r\n", tmpList.Select(x => x.Suggest).Where(x => x.TONotNullString() != "").ToList());
                            }
                            else if (row.PlanTypeCode == "2")
                            {
                                Hashtable parms = new Hashtable
                                {
                                    { "ClassDateS", row.ClassDateS },
                                    { "CourseUnitCode", row.CourseUnitCode },
                                    { "TeacherID", row.TeacherID }
                                };
                                // 其他額外欄位
                                var tmpList = dao.QueryC202M_Suggest(parms);
                                if (tmpList.Any()) objSheet.Cells[Row, i].Value = string.Join("\r\n", tmpList.Select(x => x.Suggest).Where(x => x.TONotNullString() != "").ToList());
                            }
                            else if (row.PlanTypeCode == "3")
                            {
                                Hashtable parms = new Hashtable
                                {
                                    { "ClassDateS", row.ClassDateS },
                                    { "CourseUnitCode", row.CourseUnitCode },
                                    { "TeacherID", row.TeacherID }
                                };
                                // 其他額外欄位
                                var tmpList = dao.QueryC202M_Suggest_K(parms);
                                if (tmpList.Any()) objSheet.Cells[Row, i].Value = string.Join("\r\n", tmpList.Select(x => x.Suggest).Where(x => x.TONotNullString() != "").ToList());
                            }
                        }
                        else if (DisplayFields[i - 1] == "TrainingDateS" && aPTCode23.Contains(row.PlanTypeCode))
                        {
                            objSheet.Cells[Row, i].Value = row.ClassDateS.TONotNullString();
                        }
                        else if (DisplayFields[i - 1] == "TrainingDateE" && aPTCode23.Contains(row.PlanTypeCode))
                        {
                            objSheet.Cells[Row, i].Value = row.ClassDateE.TONotNullString();
                        }
                        else if (DisplayFields[i - 1] == "TrainingTimeS" && aPTCode23.Contains(row.PlanTypeCode))
                        {
                            objSheet.Cells[Row, i].Value = row.TrainingDateS.TONotNullString();
                        }
                        else if (DisplayFields[i - 1] == "TrainingTimeE" && aPTCode23.Contains(row.PlanTypeCode))
                        {
                            objSheet.Cells[Row, i].Value = row.TrainingDateE.TONotNullString();
                        }
                        else
                        {
                            objSheet.Cells[Row, i].Value = row.GetPiValue(DisplayFields[i - 1]).TONotNullString();
                        }
                        objSheet.Cells[Row, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        objSheet.Cells[Row, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        objSheet.Cells[Row, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        objSheet.Cells[Row, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    Row++;
                }
                // 後續處理
                objSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                objSheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                objSheet.SelectedRange[1, 1, 1, DisplayCols.Length].Merge = true;
                objSheet.SelectedRange[2, 4, 2, DisplayCols.Length].Merge = true;
                objSheet.SelectedRange[4, 1, 4, DisplayCols.Length].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                objSheet.SelectedRange[4, 1, 4, DisplayCols.Length].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 242, 242));
                for (int i = 1; i <= DisplayCols.Length; i++) objSheet.Column(i).Width = 14;
                objSheet.Row(4).Height = 45;
                objSheet.Cells.Style.Font.Name = "新細明體";
                objSheet.Cells.Style.Font.Size = 12;
                objSheet.Cells.Style.WrapText = true;

                string s_FileName1 = string.Concat("匯出師資個人授課總表", "_", CommonsServices.GetDateTimeNow1());
                string s_FileName1_xlsx = string.Concat(s_FileName1, ".xlsx");
                // OK A2/C202M - 教師首頁>教師資料>個人授課資料：匯出師資個人授課總表
                FuncLogAdd.Do(ModifyEvent.NONE, ModifyType.PRINT, ModifyState.SUCCESS, "A2/C202M", null, form, s_FileName1_xlsx);
                return File(objExcel.GetAsByteArray(), HelperUtil.XLSXContentType, s_FileName1_xlsx);
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                sm.LastErrorMessage = "發生錯誤！";
            }
            return rtn;
        }

        [HttpGet]
        public ActionResult Detail(string FilterString,
            string PlanTypeCode, string PlanID, string ClassID, string CourseUnitCode,
            string Year, string ClassDateS, string TeacherID)
        {
            SessionModel sm = SessionModel.Get();
            C202MViewModel model = new C202MViewModel { Detail = new C202MDetailModel() };
            model.Detail.FilterString = FilterString;
            ActionResult rtn = View("Detail", model);
            string[] PlanTypeCodeRANGE = { "1", "2", "3" };
            if (string.IsNullOrEmpty(PlanTypeCode) || !PlanTypeCodeRANGE.Contains(PlanTypeCode)) { sm.LastErrorMessage = "查無資料"; return rtn; }
            if (PlanID.TONotNullString() == "") { sm.LastErrorMessage = "查無資料"; return rtn; }
            if (CourseUnitCode.TONotNullString() == "") { sm.LastErrorMessage = "查無資料"; return rtn; }
            model.Detail.PlanTypeCode = PlanTypeCode;
            if (PlanTypeCode == "1")
            {
                A2DAO dao = new A2DAO();
                // 補助大專校院辦理就業學程計畫
                var data = dao.GetRow(new TransData_D_Class()
                {
                    PlanID = PlanID,
                    Year = Year,
                    ClassDateS = ClassDateS,
                    CourseUnitCode = CourseUnitCode,
                    TeacherID = TeacherID,
                });
                model.Detail.InjectFrom(data);
                // 其他額外欄位
                var tmpList = dao.GetRowList(new TransData_D_Satisfy()
                {
                    PlanID = PlanID,
                    Year = Year,
                    CourseUnitCode = CourseUnitCode,
                    TeacherID = model.Detail.TeacherID,
                });
                if (tmpList.Any())
                {
                    // 授課滿意度達70%以上對象表示滿意情形
                    List<string> listSatisfy = new List<string>();
                    foreach (var row in tmpList)
                    {
                        double totalScore = row.Q1.TOInt32() + row.Q2.TOInt32() + row.Q3.TOInt32() + row.Q4.TOInt32() + row.Q5.TOInt32()
                            + row.Q6.TOInt32() + row.Q7.TOInt32() + row.Q8.TOInt32() + row.Q9.TOInt32() + row.Q10.TOInt32() + row.Q11.TOInt32();
                        double avgScore = totalScore / 11;
                        if (avgScore >= 4) listSatisfy.Add("1"); else listSatisfy.Add("0");
                    }
                    double totalSatisfy = listSatisfy.Where(x => x == "1").ToCount().ToDouble() / listSatisfy.ToCount().ToDouble();

                    model.Detail.Satisfy = $"{Math.Ceiling(totalSatisfy * 100)}%";

                    // 建議事項(意見或建議)
                    model.Detail.Suggest = string.Join("\r\n", tmpList.Select(x => x.Suggest).Where(x => x.TONotNullString() != "").ToList());
                }
            }
            if (PlanTypeCode == "2")
            {
                A2DAO dao = new A2DAO();
                //小型企業人力提升計畫//ClassID=ClassID,//TrainID = PlanID,
                var data = dao.GetRow(new TransData_S_Class()
                {
                    ClassDateS = ClassDateS,
                    CourseUnitCode = CourseUnitCode,
                    TeacherID = TeacherID,
                });
                model.Detail.InjectFrom(data);

                Hashtable parms = new Hashtable
                {
                    { "ClassDateS", ClassDateS },
                    { "CourseUnitCode", CourseUnitCode },
                    { "TeacherID", TeacherID }
                };
                // 其他額外欄位
                var tmpList = dao.QueryC202M_Suggest(parms);
                if (tmpList.Any())
                {
                    // 授課滿意度達70%以上對象表示滿意情形
                    List<string> listSatisfy = new List<string>();
                    foreach (var row in tmpList)
                    {
                        double totalScore = row.Q1.TOInt32() + row.Q2.TOInt32() + row.Q3.TOInt32() + row.Q4.TOInt32() + row.Q5.TOInt32()
                            + row.Q6.TOInt32() + row.Q7.TOInt32() + row.Q8.TOInt32() + row.Q9.TOInt32() + row.Q10.TOInt32() + row.Q11.TOInt32();
                        double avgScore = totalScore / 11;
                        if (avgScore >= 4) listSatisfy.Add("1"); else listSatisfy.Add("0");
                    }
                    double totalSatisfy = listSatisfy.Where(x => x == "1").ToCount().ToDouble() / listSatisfy.ToCount().ToDouble();

                    model.Detail.Satisfy = $"{Math.Ceiling(totalSatisfy * 100)}%";

                    // 建議事項(意見或建議)
                    model.Detail.Suggest = string.Join("\r\n", tmpList.Select(x => x.Suggest).Where(x => x.TONotNullString() != "").ToList());
                }
            }
            if (PlanTypeCode == "3")
            {
                A2DAO dao = new A2DAO();
                // 3:企業人力資源提升計畫(大人提)
                var data = dao.GetRow(new TransData_K_Class()
                {
                    ClassDateS = ClassDateS,
                    CourseUnitCode = CourseUnitCode,
                    TeacherID = TeacherID,
                });
                model.Detail.InjectFrom(data);

                Hashtable parms = new Hashtable
                {
                    { "ClassDateS", ClassDateS },
                    { "CourseUnitCode", CourseUnitCode },
                    { "TeacherID", TeacherID }
                };
                // 其他額外欄位
                var tmpList = dao.QueryC202M_Suggest_K(parms);
                if (tmpList.Any())
                {
                    // 授課滿意度達70%以上對象表示滿意情形
                    List<string> listSatisfy = new List<string>();
                    foreach (var row in tmpList)
                    {
                        double totalScore = row.Q1.TOInt32() + row.Q2.TOInt32() + row.Q3.TOInt32() + row.Q4.TOInt32() + row.Q5.TOInt32()
                            + row.Q6.TOInt32() + row.Q7.TOInt32() + row.Q8.TOInt32() + row.Q9.TOInt32() + row.Q10.TOInt32() + row.Q11.TOInt32();
                        double avgScore = totalScore / 11;
                        if (avgScore >= 4) listSatisfy.Add("1"); else listSatisfy.Add("0");
                    }
                    double totalSatisfy = listSatisfy.Where(x => x == "1").ToCount().ToDouble() / listSatisfy.ToCount().ToDouble();

                    model.Detail.Satisfy = $"{Math.Ceiling(totalSatisfy * 100)}%";

                    // 建議事項(意見或建議)
                    model.Detail.Suggest = string.Join("\r\n", tmpList.Select(x => x.Suggest).Where(x => x.TONotNullString() != "").ToList());
                }
            }
            return rtn;
        }

        [HttpPost]
        public ActionResult Detail_Back(C202MViewModel model)
        {
            // 處理 查詢頁的 查詢條件
            if (model.Detail.FilterString.TONotNullString() != "")
            {
                var Filters = model.Detail.FilterString.TONotNullString().Split(new string[] { "||" }, StringSplitOptions.None);
                if (Filters.Any() && Filters.ToCount() == 6)
                {
                    var tmpStr1 = Filters[2].Split(',').Where(x => x != "").ToList();
                    model.Form.Year = Filters[0];
                    model.Form.PlanTypeCode = Filters[1];
                    model.Form.Units = string.Join(",", tmpStr1);
                    model.Form.TrainingUnit = Filters[3];
                    model.Form.JobAbilityCode = Filters[4];
                    model.Form.CourseUnitCode = Filters[5];
                }
            }
            return Index(model.Form);
        }
    }
}