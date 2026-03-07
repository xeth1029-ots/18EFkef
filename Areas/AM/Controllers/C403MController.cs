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
using WKEFSERVICE.Models.Entities;
using System.Collections.Generic;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C403MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C403MViewModel model = new C403MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C403MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            ShareCodeListModel sclm = new ShareCodeListModel();
            ActionResult rtn = View("Index", form);
            if (form.Year.TONotNullString() == "") { sm.LastErrorMessage = "請選擇計畫年度！"; return rtn; }
            AMDAO dao = new AMDAO();
            // 暫存資料 老師資料表 // var teacherDatas = dao.GetRowList(new Teacher() { Online = "Y" }).ToList();
            Teacher whereTeacher = new Teacher();
            if (form.TeacherUnit.TONotNullString() != "" && form.TeacherUnit.TONotNullString() != "0")
            {
                whereTeacher = new Teacher() { UnitCode = form.TeacherUnit };
            }
            //教師姓名
            if (!string.IsNullOrEmpty(form.TeacherName)) { whereTeacher.TeacherName = form.TeacherName; }

            var lstTeachers = dao.GetRowList(whereTeacher).ToList();
            //if (form.TeacherUnit.TONotNullString() != "" && form.TeacherUnit.TONotNullString() != "0")
            //    teacherDatas = teacherDatas.Where(x => x.UnitCode == form.TeacherUnit.TONotNullString()).ToList();
            //string lst2A = "\n";//test
            //int iRow2 = 0;//test
            try
            {
                #region 匯出程序
                // 建立 Excel 物件
                ExcelPackage objExcel = new ExcelPackage();
                //滿意度調查表(大專就業學程)
                if (form.PlanTypeCode == "0" || form.PlanTypeCode == "1")
                {
                    ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add("滿意度調查表(大專就業學程)");
                    #region 滿意度調查表(大專就業學程)
                    // 產生標題列
                    objSheet.Cells[1, 1].Value = (form.Year.TOInt32() - 1911).ToString() + " 年度關鍵就業力課程滿意度調查 - 補助大專校院辦理就業學程計畫";
                    objSheet.SelectedRange[1, 1, 1, 26].Merge = true;
                    objSheet.Cells[2, 1].Value = "講師姓名";
                    objSheet.Cells[2, 2].Value = "所屬轄區";
                    objSheet.Cells[2, 3].Value = "授課內容所署分署";
                    objSheet.Cells[2, 4].Value = "學校名稱";
                    objSheet.Cells[2, 5].Value = "計畫序號";
                    objSheet.Cells[2, 6].Value = "學程名稱";
                    objSheet.Cells[2, 7].Value = "關鍵職能類別";
                    objSheet.Cells[2, 8].Value = "課程名稱(單元)";
                    objSheet.Cells[2, 9].Value = "課程起日";
                    objSheet.Cells[2, 10].Value = "課程訖日";
                    objSheet.Cells[2, 11].Value = "學員姓名";
                    objSheet.Cells[2, 12].Value = "科系";
                    objSheet.Cells[2, 13].Value = "身分";
                    objSheet.Cells[2, 14].Value = "填寫日期";
                    objSheet.Cells[2, 15].Value = "1.課程內容對我未來投入職場有幫助";
                    objSheet.Cells[2, 16].Value = "2.課程安排對我學習有幫助";
                    objSheet.Cells[2, 17].Value = "3.課程規劃對我職場能力的提升有幫助";
                    objSheet.Cells[2, 18].Value = "4.教學態度良好";
                    objSheet.Cells[2, 19].Value = "5.課堂互動良好";
                    objSheet.Cells[2, 20].Value = "6.表達及回答問題的能力良好";
                    objSheet.Cells[2, 21].Value = "7.教材符合授課內容";
                    objSheet.Cells[2, 22].Value = "8.教材有助於課程學習";
                    objSheet.Cells[2, 23].Value = "9.整體課程與教學能讓我保持專注";
                    objSheet.Cells[2, 24].Value = "10.我能充分吸收課程所教授的知識與技能";
                    objSheet.Cells[2, 25].Value = "11.課程所學對我未來工作很有幫助";
                    objSheet.Cells[2, 26].Value = "建議事項(意見或建議)";
                    // 開始填資料
                    var mainDatas_D = dao.QueryC403M1(form.Year.TONotNullString());
                    //var mainDatas_D = dao.GetRowList(new TransData_D_Satisfy() { Year = form.Year.TONotNullString() });
                    //if (form.TeacherUnit.TONotNullString() != "" && form.TeacherUnit.TONotNullString() != "0")
                    //    mainDatas_D = mainDatas_D.Where(x => x.UnitCode == form.TeacherUnit.TONotNullString()).ToList();
                    int iRow = 2;
                    if (mainDatas_D.Any())
                    {
                        // 主資料 排序
                        mainDatas_D = mainDatas_D.OrderBy(x => x.PlanID).ThenBy(x => x.CourseUnitCode).ToList();
                        foreach (var row in mainDatas_D)
                        {
                            // 老師資料
                            var findTeacher = lstTeachers.Where(x => x.IDNO == row.TeacherID).Where(x => CommonsServices.CheckOfflineDateYears(x.OfflineDate, form.Year)).FirstOrDefault();
                            if (findTeacher == null) continue;
                            if (findTeacher != null)
                            {
                                iRow++;
                                objSheet.Cells[iRow, 1].Value = findTeacher.TeacherName;
                                objSheet.Cells[iRow, 2].Value = sclm.UNIT_List.Where(x => x.Value == findTeacher.UnitCode).FirstOrDefault().Text;
                            }
                            // 課程資料 (大專)
                            var classDatas = dao.GetRowList(new TransData_D_Class()
                            {
                                PlanID = row.PlanID,
                                Year = row.Year,
                                //ClassDateS = row.ClassDateS,
                                CourseUnitCode = row.CourseUnitCode,
                                TeacherID = row.TeacherID,
                            }).FirstOrDefault();
                            if (classDatas != null)
                            {
                                objSheet.Cells[iRow, 3].Value = sclm.UNIT_List.Where(x => x.Value == classDatas.UnitCode).FirstOrDefault().Text;
                                objSheet.Cells[iRow, 4].Value = classDatas.SchoolName;
                                objSheet.Cells[iRow, 5].Value = classDatas.PlanID;
                                objSheet.Cells[iRow, 6].Value = classDatas.ClassName;
                                objSheet.Cells[iRow, 7].Value = classDatas.JobAbilityCode;
                                objSheet.Cells[iRow, 8].Value = classDatas.CourseUnitCode;
                                objSheet.Cells[iRow, 9].Value = classDatas.ClassDateS;
                                objSheet.Cells[iRow, 10].Value = classDatas.ClassDateE;
                            }
                            objSheet.Cells[iRow, 11].Value = row.StudentName;
                            objSheet.Cells[iRow, 12].Value = row.Dept;
                            objSheet.Cells[iRow, 13].Value = row.Identity;
                            objSheet.Cells[iRow, 14].Value = row.FillDate;
                            for (int i = 1; i <= 11; i++)
                            {
                                objSheet.Cells[iRow, 14 + i].Value = row.GetPiValue("Q" + i.ToString()).TONotNullString();
                            }
                            objSheet.Cells[iRow, 26].Value = row.Suggest;
                        }
                    }
                    // 後續處理
                    objSheet.Row(2).Height = 90;
                    objSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    objSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    objSheet.SelectedRange[2, 1, 2, 26].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    objSheet.SelectedRange[2, 1, 2, 26].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 242, 242));
                    objSheet.Cells[1, 1, iRow, 26].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 26].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 26].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 26].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells.Style.Font.Name = "新細明體";
                    objSheet.Cells.Style.Font.Size = 12;
                    objSheet.Cells.Style.WrapText = true;
                    #endregion
                }
                //滿意度調查表(小型企業人力提升計畫)
                if (form.PlanTypeCode == "0" || form.PlanTypeCode == "2")
                {
                    ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add("滿意度調查表(小型企業人力提升計畫)");
                    #region 滿意度調查表(小型企業人力提升計畫)
                    // 產生標題列
                    objSheet.Cells[1, 1].Value = (form.Year.TOInt32() - 1911).ToString() + " 年度關鍵就業力課程滿意度調查-小型企業人力提升計畫";
                    objSheet.SelectedRange[1, 1, 1, 27].Merge = true;
                    objSheet.Cells[2, 1].Value = "講師姓名";
                    objSheet.Cells[2, 2].Value = "所屬轄區";
                    objSheet.Cells[2, 3].Value = "授課內容所署分署";
                    objSheet.Cells[2, 4].Value = "訓練案號";
                    objSheet.Cells[2, 5].Value = "參訓企業";
                    objSheet.Cells[2, 6].Value = "訓練單位";
                    objSheet.Cells[2, 7].Value = "課程名稱";
                    objSheet.Cells[2, 8].Value = "內訓 / 外訓";
                    objSheet.Cells[2, 9].Value = "關鍵職能類別";
                    objSheet.Cells[2, 10].Value = "課程起日";
                    objSheet.Cells[2, 11].Value = "課程訖日";
                    objSheet.Cells[2, 12].Value = "學員姓名";
                    objSheet.Cells[2, 13].Value = "個人工作經驗";
                    objSheet.Cells[2, 14].Value = "職務別";
                    objSheet.Cells[2, 15].Value = "填寫日期";
                    objSheet.Cells[2, 16].Value = "1.課程內容符合我在職場上所需的知識與技能";
                    objSheet.Cells[2, 17].Value = "2.課程難易的安排，對我有效學習有所幫助";
                    objSheet.Cells[2, 18].Value = "3.課程內容有助於我完備職場就業能力";
                    objSheet.Cells[2, 19].Value = "4.我滿意授課講師的教學態度";
                    objSheet.Cells[2, 20].Value = "5.我滿意授課講師的教學方式";
                    objSheet.Cells[2, 21].Value = "6.我認同講師授課的專業度";
                    objSheet.Cells[2, 22].Value = "7.我同意教材能符合授課內容";
                    objSheet.Cells[2, 23].Value = "8.我同意教材能輔助課程學習";
                    objSheet.Cells[2, 24].Value = "9.在訓練過程中，整體課程與教學能讓我保持專注";
                    objSheet.Cells[2, 25].Value = "10.在完成訓練後，我能習得課程所教授的知識與技能";
                    objSheet.Cells[2, 26].Value = "11.在完成訓練後，課程所學對目前或未來工作的表現有幫助";
                    objSheet.Cells[2, 27].Value = "建議事項(意見或建議)";

                    // 開始填資料
                    var mainDatas_S = dao.QueryC403M2(form.Year.TONotNullString());
                    //var mainDatas_S = dao.GetRowList(new TransData_S_Satisfy() { Year = form.Year.TONotNullString() });
                    //if (form.TeacherUnit.TONotNullString() != "" && form.TeacherUnit.TONotNullString() != "0")
                    //    mainDatas_S = mainDatas_S.Where(x => x.UnitCode == form.TeacherUnit.TONotNullString()).ToList();
                    int iRow = 2;
                    if (mainDatas_S.Any())
                    {
                        // 主資料 排序
                        mainDatas_S = mainDatas_S.OrderBy(x => x.TrainID).ThenBy(x => x.ClassID).ThenBy(x => x.CourseUnitCode).ToList();
                        foreach (var row in mainDatas_S)
                        {
                            // 老師資料
                            var findTeacher = lstTeachers.Where(x => x.IDNO == row.TeacherID).Where(x => CommonsServices.CheckOfflineDateYears(x.OfflineDate, form.Year)).FirstOrDefault();
                            if (findTeacher == null) continue;
                            if (findTeacher != null)
                            {
                                iRow++;
                                objSheet.Cells[iRow, 1].Value = findTeacher.TeacherName;
                                objSheet.Cells[iRow, 2].Value = sclm.UNIT_List.Where(x => x.Value == findTeacher.UnitCode).FirstOrDefault().Text;
                            }
                            // 課程資料 (小人提)
                            var classDatas = dao.GetRowList(new TransData_S_Class()
                            {
                                TrainID = row.TrainID,
                                ClassID = row.ClassID,
                                //ClassDateS = row.ClassDateS,
                                CourseUnitCode = row.CourseUnitCode,
                                TeacherID = row.TeacherID,
                            }).FirstOrDefault();
                            if (classDatas != null)
                            {
                                objSheet.Cells[iRow, 3].Value = classDatas.UnitName;
                                objSheet.Cells[iRow, 4].Value = classDatas.TrainID;
                                objSheet.Cells[iRow, 5].Value = classDatas.JoinCompany;
                                objSheet.Cells[iRow, 6].Value = classDatas.TrainingUnit;
                                objSheet.Cells[iRow, 7].Value = classDatas.ClassName;
                                objSheet.Cells[iRow, 8].Value = classDatas.TrainingType;
                                objSheet.Cells[iRow, 9].Value = classDatas.JobAbilityCode;
                                objSheet.Cells[iRow, 10].Value = classDatas.ClassDateS;
                                objSheet.Cells[iRow, 11].Value = classDatas.ClassDateE;
                            }
                            objSheet.Cells[iRow, 12].Value = row.StudentName;
                            objSheet.Cells[iRow, 13].Value = row.PersonWorkExp;
                            objSheet.Cells[iRow, 14].Value = row.DutyType;
                            objSheet.Cells[iRow, 15].Value = row.FillDate;
                            for (int i = 1; i <= 11; i++)
                            {
                                objSheet.Cells[iRow, 15 + i].Value = row.GetPiValue("Q" + i.ToString()).TONotNullString();
                            }
                            objSheet.Cells[iRow, 27].Value = row.Suggest;

                            //test
                            //if (findTeacher.UnitCode == "1" && row.CourseUnitCode.StartsWith("KC")) {
                            //    iRow2 += 1;
                            //    lst2A += string.Concat(iRow2, "\t-", findTeacher.UnitCode, "-", row.CourseUnitCode, "-", row.TeacherID, "-", row.TeacherName, "-", row.StudentID, "-", row.StudentName, "\n");
                            //}
                            //test
                        }
                    }
                    //LOG.Debug(string.Concat("##xx-lst2A\n", lst2A));//test
                    // 後續處理
                    objSheet.Row(2).Height = 135;
                    objSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    objSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    objSheet.SelectedRange[2, 1, 2, 27].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    objSheet.SelectedRange[2, 1, 2, 27].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 242, 242));
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells.Style.Font.Name = "新細明體";
                    objSheet.Cells.Style.Font.Size = 12;
                    objSheet.Cells.Style.WrapText = true;
                    #endregion
                }
                //大人提,企業人力資源提升計畫
                if (form.PlanTypeCode == "0" || form.PlanTypeCode == "3")
                {
                    ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add("滿意度調查表(企業人力資源提升計畫)");
                    #region 滿意度調查表(企業人力資源提升計畫)
                    // 產生標題列
                    objSheet.Cells[1, 1].Value = $"{form.Year.TOInt32() - 1911} 年度關鍵就業力課程滿意度調查-企業人力資源提升計畫";
                    objSheet.SelectedRange[1, 1, 1, 27].Merge = true;
                    objSheet.Cells[2, 1].Value = "講師姓名";
                    objSheet.Cells[2, 2].Value = "所屬轄區";
                    objSheet.Cells[2, 3].Value = "授課內容所署分署";
                    objSheet.Cells[2, 4].Value = "訓練案號";
                    objSheet.Cells[2, 5].Value = "參訓企業";
                    objSheet.Cells[2, 6].Value = "訓練單位";
                    objSheet.Cells[2, 7].Value = "課程名稱";
                    objSheet.Cells[2, 8].Value = "內訓 / 外訓";
                    objSheet.Cells[2, 9].Value = "關鍵職能類別";
                    objSheet.Cells[2, 10].Value = "課程起日";
                    objSheet.Cells[2, 11].Value = "課程訖日";
                    objSheet.Cells[2, 12].Value = "學員姓名";
                    objSheet.Cells[2, 13].Value = "個人工作經驗";
                    objSheet.Cells[2, 14].Value = "職務別";
                    objSheet.Cells[2, 15].Value = "填寫日期";
                    objSheet.Cells[2, 16].Value = "1.課程內容符合我在職場上所需的知識與技能";
                    objSheet.Cells[2, 17].Value = "2.課程難易的安排，對我有效學習有所幫助";
                    objSheet.Cells[2, 18].Value = "3.課程內容有助於我完備職場就業能力";
                    objSheet.Cells[2, 19].Value = "4.我滿意授課講師的教學態度";
                    objSheet.Cells[2, 20].Value = "5.我滿意授課講師的教學方式";
                    objSheet.Cells[2, 21].Value = "6.我認同講師授課的專業度";
                    objSheet.Cells[2, 22].Value = "7.我同意教材能符合授課內容";
                    objSheet.Cells[2, 23].Value = "8.我同意教材能輔助課程學習";
                    objSheet.Cells[2, 24].Value = "9.在訓練過程中，整體課程與教學能讓我保持專注";
                    objSheet.Cells[2, 25].Value = "10.在完成訓練後，我能習得課程所教授的知識與技能";
                    objSheet.Cells[2, 26].Value = "11.在完成訓練後，課程所學對目前或未來工作的表現有幫助";
                    objSheet.Cells[2, 27].Value = "建議事項(意見或建議)";

                    // 開始填資料
                    var mainDatas_K = dao.QueryC403M3($"{form.Year}");
                    //var mainDatas_S = dao.GetRowList(new TransData_S_Satisfy() { Year = form.Year.TONotNullString() });
                    //if (form.TeacherUnit.TONotNullString() != "" && form.TeacherUnit.TONotNullString() != "0")
                    //    mainDatas_S = mainDatas_S.Where(x => x.UnitCode == form.TeacherUnit.TONotNullString()).ToList();
                    int iRow = 2;
                    if (mainDatas_K.Any())
                    {
                        // 主資料 排序
                        mainDatas_K = mainDatas_K.OrderBy(x => x.TrainID).ThenBy(x => x.ClassID).ThenBy(x => x.CourseUnitCode).ToList();
                        foreach (var row in mainDatas_K)
                        {
                            // 老師資料
                            var findTeacher = lstTeachers.Where(x => x.IDNO == row.TeacherID).Where(x => CommonsServices.CheckOfflineDateYears(x.OfflineDate, form.Year)).FirstOrDefault();
                            if (findTeacher == null) continue;
                            if (findTeacher != null)
                            {
                                iRow++;
                                objSheet.Cells[iRow, 1].Value = findTeacher.TeacherName;
                                objSheet.Cells[iRow, 2].Value = sclm.UNIT_List.Where(x => x.Value == findTeacher.UnitCode).FirstOrDefault().Text;
                            }
                            // 課程資料 (小人提)
                            var classDatas = dao.GetRowList(new TransData_K_Class()
                            {
                                TrainID = row.TrainID,
                                ClassID = row.ClassID,
                                //ClassDateS = row.ClassDateS,
                                CourseUnitCode = row.CourseUnitCode,
                                TeacherID = row.TeacherID,
                            }).FirstOrDefault();
                            if (classDatas != null)
                            {
                                objSheet.Cells[iRow, 3].Value = classDatas.UnitName;
                                objSheet.Cells[iRow, 4].Value = classDatas.TrainID;
                                objSheet.Cells[iRow, 5].Value = classDatas.JoinCompany;
                                objSheet.Cells[iRow, 6].Value = classDatas.TrainingUnit;
                                objSheet.Cells[iRow, 7].Value = classDatas.ClassName;
                                objSheet.Cells[iRow, 8].Value = classDatas.TrainingType;
                                objSheet.Cells[iRow, 9].Value = classDatas.JobAbilityCode;
                                objSheet.Cells[iRow, 10].Value = classDatas.ClassDateS;
                                objSheet.Cells[iRow, 11].Value = classDatas.ClassDateE;
                            }
                            objSheet.Cells[iRow, 12].Value = row.StudentName;
                            objSheet.Cells[iRow, 13].Value = row.PersonWorkExp;
                            objSheet.Cells[iRow, 14].Value = row.DutyType;
                            objSheet.Cells[iRow, 15].Value = row.FillDateStr;
                            for (int i = 1; i <= 11; i++)
                            {
                                objSheet.Cells[iRow, 15 + i].Value = row.GetPiValue($"Q{i}").TONotNullString();
                            }
                            objSheet.Cells[iRow, 27].Value = row.Suggest;

                            //test
                            //if (findTeacher.UnitCode == "1" && row.CourseUnitCode.StartsWith("KC")) {
                            //    iRow2 += 1;
                            //    lst2A += string.Concat(iRow2, "\t-", findTeacher.UnitCode, "-", row.CourseUnitCode, "-", row.TeacherID, "-", row.TeacherName, "-", row.StudentID, "-", row.StudentName, "\n");
                            //}
                            //test
                        }
                    }
                    // 後續處理,LOG.Debug(string.Concat("##xx-lst2A\n", lst2A));//test 
                    objSheet.Row(2).Height = 135;
                    objSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    objSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    objSheet.SelectedRange[2, 1, 2, 27].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    objSheet.SelectedRange[2, 1, 2, 27].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 242, 242));
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[1, 1, iRow, 27].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells.Style.Font.Name = "新細明體";
                    objSheet.Cells.Style.Font.Size = 12;
                    objSheet.Cells.Style.WrapText = true;
                    #endregion
                }

                string s_FileName1 = $"滿意度調查表_{CommonsServices.GetDateTimeNow1()}";
                string s_FileName1_xlsx = $"{s_FileName1}.xlsx";
                // OK
                FuncLogAdd.Do(ModifyEvent.NONE, ModifyType.PRINT, ModifyState.SUCCESS, "AM/C403M", null, form, s_FileName1 + (form.ExportFormat == "0" ? ".xlsx" : ".ods"));
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
                #endregion
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