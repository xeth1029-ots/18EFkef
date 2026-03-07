using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.A3.Models;
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
using System.Collections;

namespace WKEFSERVICE.Areas.A3.Controllers
{
    public class C502MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C502MViewModel model = new C502MViewModel();
            model.Form.TeacherUnit = sm.UserInfo.User.UNITID;
            return View(model.Form);
        }

        /// <summary>師資授課情形統計表</summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(C502MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            ShareCodeListModel sclm = new ShareCodeListModel();
            ActionResult rtn = View("Index", form);
            form.TeacherUnit = sm.UserInfo.User.UNITID;
            if (form.Year.TONotNullString() == "") { sm.LastErrorMessage = "請選擇計畫年度！"; return rtn; }
            // 查詢 主資料 Teacher
            A3DAO dao = new A3DAO();
            var lstTeachers = dao.GetRowList(new Teacher() { Online = "Y" }).ToList();
            if (sm.UserInfo.LoginCharacter != "4") lstTeachers = lstTeachers.Where(x => x.UnitCode == sm.UserInfo.User.UNITID).ToList();
            if (lstTeachers.ToCount() <= 0) { sm.LastResultMessage = "查無資料"; return rtn; }
            try
            {
                #region 匯出程序
                // 建立 Excel 物件
                ExcelPackage objExcel = new ExcelPackage();
                ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add("關鍵就業力課程授課情形統計表");

                // 產生標題列 含固定字串
                #region 產生標題列 含固定字串

                int iLastCol = 46;
                objSheet.Cells[1, 1].Value = (form.Year.TOInt32() - 1911).ToString() + " 年度關鍵就業力課程師資授課情形統計表";
                objSheet.SelectedRange[1, 1, 1, iLastCol].Merge = true;
                objSheet.Cells[2, 1].Value = "轄區";
                objSheet.SelectedRange[2, 1, 4, 1].Merge = true;
                objSheet.Cells[2, 2].Value = "課程總師資";
                objSheet.SelectedRange[2, 2, 2, 7].Merge = true;
                objSheet.Cells[2, 8].Value = "授課時數";
                objSheet.SelectedRange[2, 8, 2, 20].Merge = true;
                objSheet.Cells[2, 21].Value = "授課對象針對師資授課表示滿意之人數達70%以上"; //"授課滿意度達70%以上對象表示滿意情形";
                objSheet.SelectedRange[2, 21, 2, 33].Merge = true;
                objSheet.Cells[2, 34].Value = "滿意度填答人數";
                objSheet.SelectedRange[2, 34, 2, 46].Merge = true;
                objSheet.Cells[3, 2].Value = "師資數";
                objSheet.SelectedRange[3, 2, 4, 2].Merge = true;
                objSheet.Cells[3, 3].Value = "可授課職類";
                objSheet.SelectedRange[3, 3, 3, 5].Merge = true;
                objSheet.Cells[3, 6].Value = "類別";
                objSheet.SelectedRange[3, 6, 3, 7].Merge = true;
                objSheet.Cells[3, 8].Value = "補助大專校院辦理就業學程計畫";
                objSheet.SelectedRange[3, 8, 3, 11].Merge = true;
                objSheet.Cells[3, 12].Value = "小型企業人力提升計畫";
                objSheet.SelectedRange[3, 12, 3, 15].Merge = true;
                objSheet.Cells[3, 16].Value = "企業人力資源提升計畫";
                objSheet.SelectedRange[3, 16, 3, 19].Merge = true;

                objSheet.Cells[3, 20].Value = "授課總時數";
                objSheet.SelectedRange[3, 20, 4, 20].Merge = true;
                objSheet.Cells[3, 21].Value = "補助大專校院辦理就業學程計畫";
                objSheet.SelectedRange[3, 21, 3, 24].Merge = true;
                objSheet.Cells[3, 25].Value = "小型企業人力提升計畫";
                objSheet.SelectedRange[3, 25, 3, 28].Merge = true;
                objSheet.Cells[3, 29].Value = "企業人力資源提升計畫";
                objSheet.SelectedRange[3, 29, 3, 32].Merge = true;
                objSheet.Cells[3, 33].Value = "整體";
                objSheet.SelectedRange[3, 33, 4, 33].Merge = true;
                objSheet.Cells[3, 34].Value = "補助大專校院辦理就業學程計畫";
                objSheet.SelectedRange[3, 34, 3, 37].Merge = true;
                objSheet.Cells[3, 38].Value = "小型企業人力提升計畫";
                objSheet.SelectedRange[3, 38, 3, 41].Merge = true;
                objSheet.Cells[3, 42].Value = "企業人力資源提升計畫";
                objSheet.SelectedRange[3, 42, 3, 45].Merge = true;
                objSheet.Cells[3, 46].Value = "填答總人數";
                objSheet.SelectedRange[3, 46, 4, 46].Merge = true;

                int iCol = 3;
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "學術界"; iCol++;
                objSheet.Cells[4, iCol].Value = "產業界"; iCol++;
                //補助大專校院辦理就業學程計畫			
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //小型企業人力提升計畫			
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //企業人力資源提升計畫			
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //授課總時數 
                iCol++;
                //補助大專校院辦理就業學程計畫	
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //小型企業人力提升計畫			
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //企業人力資源提升計畫		
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //整體
                iCol++;
                //補助大專校院辦理就業學程計畫	
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //小型企業人力提升計畫	
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;
                //企業人力資源提升計畫	
                objSheet.Cells[4, iCol].Value = "DC"; iCol++;
                objSheet.Cells[4, iCol].Value = "BC"; iCol++;
                objSheet.Cells[4, iCol].Value = "KC"; iCol++;
                objSheet.Cells[4, iCol].Value = "合計"; iCol++;

                int tmpIdx = 5;
                if (sm.UserInfo.LoginCharacter == "4" || sm.UserInfo.User.UNITID == "1") { objSheet.Cells[tmpIdx, 1].Value = "北分署"; tmpIdx++; }
                if (sm.UserInfo.LoginCharacter == "4" || sm.UserInfo.User.UNITID == "2") { objSheet.Cells[tmpIdx, 1].Value = "桃分署"; tmpIdx++; }
                if (sm.UserInfo.LoginCharacter == "4" || sm.UserInfo.User.UNITID == "3") { objSheet.Cells[tmpIdx, 1].Value = "中分署"; tmpIdx++; }
                if (sm.UserInfo.LoginCharacter == "4" || sm.UserInfo.User.UNITID == "4") { objSheet.Cells[tmpIdx, 1].Value = "南分署"; tmpIdx++; }
                if (sm.UserInfo.LoginCharacter == "4" || sm.UserInfo.User.UNITID == "5") { objSheet.Cells[tmpIdx, 1].Value = "高分署"; tmpIdx++; }
                if (sm.UserInfo.LoginCharacter == "4") objSheet.Cells[tmpIdx, 1].Value = "總計";
                #endregion

                // 統計用暫存
                List<int> TheSum = new List<int>();
                List<int> TheSum_X = new List<int>();
                List<int> TheSum_X2 = new List<int>();
                for (int i = 2; i <= iLastCol; i++) { TheSum.Add(0); TheSum_X.Add(0); TheSum_X2.Add(0); }
                // 統計用暫存 for DC, BC, KC
                List<double> TheSum_DBK_Good = new List<double>();  // 最後一行統計的 學員滿意
                List<double> TheSum_DBK_Fill = new List<double>();  // 最後一行統計的 學員已填
                for (int i = 1; i <= 9; i++) { TheSum_DBK_Good.Add(0); TheSum_DBK_Fill.Add(0); }

                //非系統管理者(使用) //開始填資料
                tmpIdx = 4;  // 起始行 (Row: 5-1=4)
                for (int tmpUNITID = 1; tmpUNITID <= 5; tmpUNITID++)
                {
                    //非系統管理者，只印自已的分署資料
                    if (sm.UserInfo.LoginCharacter != "4" && tmpUNITID.ToString() != sm.UserInfo.User.UNITID) continue;
                    if (form.TeacherUnit.TONotNullString() != "" && form.TeacherUnit.TONotNullString() != "0" && form.TeacherUnit.TONotNullString() != tmpUNITID.ToString()) continue;

                    var tmpTeachers = lstTeachers.Where(x => x.UnitCode == tmpUNITID.ToString()).Where(x => CommonsServices.CheckOfflineDateYears(x.OfflineDate, form.Year)).ToList();
                    //系統管理者，列印全部分署資料 //(非)自已的分署資料
                    int iRow = tmpIdx + ((sm.UserInfo.LoginCharacter == "4") ? tmpUNITID : 1);

                    objSheet.Cells[iRow, 2].Value = tmpTeachers.ToCount();
                    objSheet.Cells[iRow, 3].Value = tmpTeachers.Where(x => x.TeachJobAbilityDC == "Y").ToCount();
                    objSheet.Cells[iRow, 4].Value = tmpTeachers.Where(x => x.TeachJobAbilityBC == "Y").ToCount();
                    objSheet.Cells[iRow, 5].Value = tmpTeachers.Where(x => x.TeachJobAbilityKC == "Y").ToCount();
                    var tmpList1 = sclm.IndustryAcademicType_List().Where(x => x.Text.ToLeft(3) == "學術界").Select(s => s.Value).ToArray();
                    var tmpList2 = sclm.IndustryAcademicType_List().Where(x => x.Text.ToLeft(3) == "產業界").Select(s => s.Value).ToArray();
                    objSheet.Cells[iRow, 6].Value = tmpTeachers.Where(x => tmpList1.Contains(x.IndustryAcademicType)).ToCount();
                    objSheet.Cells[iRow, 7].Value = tmpTeachers.Where(x => tmpList2.Contains(x.IndustryAcademicType)).ToCount();
                    int DCBCKC_Sum_Hour = 0;     // 授課時數 - 授課總時數
                    int DCBCKC_Sum_Hour_x = 0;   //大小人提都有相同授課時間且為外訓時 外訓授課時數 
                    double DCBCKC_Sum_Good = 0;  // 授課滿意度達70%以上對象表示滿意情形 - [整體]
                    double DCBCKC_Sum_Fill = 0;  // 滿意度填答人數 - [整體]
                    var tmpTeachers_IDNO = tmpTeachers.Select(x => x.IDNO).Distinct().ToList();  // 抓一個老師 IDNO 清單出來查詢用

                    // 補助大專校院辦理就業學程計畫
                    if (form.PlanTypeCode == "0" || form.PlanTypeCode == "1")
                    {
                        // 取出介接資料，條件：年度+該登入者底下的老師 (分署就是分署底下的老師清單，管理者就是全部老師)
                        Hashtable HtACYear = new Hashtable { ["ACYear"] = form.Year };
                        var list_D_Class = dao.QueryForListAll<tbTransData_D_Class>("A3.Get_TransData_D_Class_TeachingHours", HtACYear).Where(x => tmpTeachers_IDNO.Contains(x.TeacherID)).ToList();
                        //var list_D_Class = dao.GetRowList(new TransData_D_Class() { Year = form.Year.TONotNullString() }).Where(x => tmpTeachers_IDNO.Contains(x.TeacherID)).ToList();

                        // 授課時數
                        objSheet.Cells[iRow, 8].Value = list_D_Class.Where(x => x.JobAbilityCode == "DC").Sum(x => x.TeachHours) ?? 0;
                        objSheet.Cells[iRow, 9].Value = list_D_Class.Where(x => x.JobAbilityCode == "BC").Sum(x => x.TeachHours) ?? 0;
                        objSheet.Cells[iRow, 10].Value = list_D_Class.Where(x => x.JobAbilityCode == "KC").Sum(x => x.TeachHours) ?? 0;
                        objSheet.Cells[iRow, 11].Value = objSheet.Cells[iRow, 8].Value.TOInt32() + objSheet.Cells[iRow, 9].Value.TOInt32() + objSheet.Cells[iRow, 10].Value.TOInt32();
                        DCBCKC_Sum_Hour = DCBCKC_Sum_Hour + objSheet.Cells[iRow, 11].Value.TOInt32();
                        // 授課滿意度達70%以上對象表示滿意情形
                        double Good_DC_Sum = 0;  // DC 本身的加總，滿意的學員數
                        double Good_BC_Sum = 0;  // BC 本身的加總，滿意的學員數
                        double Good_KC_Sum = 0;  // KC 本身的加總，滿意的學員數
                        double Fill_DC_Sum = 0;  // DC 本身的加總，總學員數
                        double Fill_BC_Sum = 0;  // BC 本身的加總，總學員數
                        double Fill_KC_Sum = 0;  // KC 本身的加總，總學員數
                        double Good_All_Total = 0;  // [合計] 的加總用，滿意的學員數
                        double Fill_All_Total = 0;  // [合計] 的加總用，總學員數
                        foreach (var rowD in list_D_Class.Where(x => x.JobAbilityCode == "DC"))
                        {
                            // 列出該課程下總學員
                            var list_D_Satisfy = dao.GetRowList(new TransData_D_Satisfy()
                            {
                                PlanID = rowD.PlanID,
                                Year = rowD.Year,
                                CourseUnitCode = rowD.CourseUnitCode,
                                TeacherID = rowD.TeacherID,
                            }).ToList();
                            // 列出滿意的學員
                            var list_D_Satisfy_Good = list_D_Satisfy.Where(x =>
                                (
                                x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble() +
                                x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()
                                ) / 11 >= 4
                            ).ToList();
                            Good_DC_Sum = Good_DC_Sum + list_D_Satisfy_Good.ToCount();
                            Fill_DC_Sum = Fill_DC_Sum + list_D_Satisfy.ToCount();
                        }
                        var tmpPa_DC = Math.Ceiling(Good_DC_Sum / Fill_DC_Sum * 100);
                        if (double.IsNaN(tmpPa_DC)) tmpPa_DC = 0;
                        objSheet.Cells[iRow, 21].Value = string.Concat(tmpPa_DC, "%");
                        objSheet.Cells[iRow, 34].Value = Fill_DC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_DC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_DC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[0] = TheSum_DBK_Good[0] + Good_DC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[0] = TheSum_DBK_Fill[0] + Fill_DC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        foreach (var rowD in list_D_Class.Where(x => x.JobAbilityCode == "BC"))
                        {
                            // 列出該課程下總學員
                            var list_D_Satisfy = dao.GetRowList(new TransData_D_Satisfy()
                            {
                                PlanID = rowD.PlanID,
                                Year = rowD.Year,
                                CourseUnitCode = rowD.CourseUnitCode,
                                TeacherID = rowD.TeacherID,
                            }).ToList();
                            // 列出滿意的學員
                            var list_D_Satisfy_Good = list_D_Satisfy.Where(x =>
                                (
                                x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble() +
                                x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()
                                ) / 11 >= 4
                            ).ToList();
                            Good_BC_Sum = Good_BC_Sum + list_D_Satisfy_Good.ToCount();
                            Fill_BC_Sum = Fill_BC_Sum + list_D_Satisfy.ToCount();
                        }
                        var tmpPa_BC = Math.Ceiling(Good_BC_Sum / Fill_BC_Sum * 100);
                        if (double.IsNaN(tmpPa_BC)) tmpPa_BC = 0;
                        objSheet.Cells[iRow, 22].Value = string.Concat(tmpPa_BC, "%");
                        objSheet.Cells[iRow, 35].Value = Fill_BC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_BC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_BC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[1] = TheSum_DBK_Good[1] + Good_BC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[1] = TheSum_DBK_Fill[1] + Fill_BC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        foreach (var rowD in list_D_Class.Where(x => x.JobAbilityCode == "KC"))
                        {
                            // 列出該課程下總學員
                            var list_D_Satisfy = dao.GetRowList(new TransData_D_Satisfy()
                            {
                                PlanID = rowD.PlanID,
                                Year = rowD.Year,
                                CourseUnitCode = rowD.CourseUnitCode,
                                TeacherID = rowD.TeacherID,
                            }).ToList();
                            // 列出滿意的學員
                            var list_D_Satisfy_Good = list_D_Satisfy.Where(x =>
                                (
                                x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble() +
                                x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()
                                ) / 11 >= 4
                            ).ToList();
                            Good_KC_Sum = Good_KC_Sum + list_D_Satisfy_Good.ToCount();
                            Fill_KC_Sum = Fill_KC_Sum + list_D_Satisfy.ToCount();
                        }
                        var tmpPa_KC = Math.Ceiling(Good_KC_Sum / Fill_KC_Sum * 100);
                        if (double.IsNaN(tmpPa_KC)) tmpPa_KC = 0;
                        objSheet.Cells[iRow, 23].Value = string.Concat(tmpPa_KC, "%");
                        objSheet.Cells[iRow, 36].Value = Fill_KC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_KC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_KC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[2] = TheSum_DBK_Good[2] + Good_KC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[2] = TheSum_DBK_Fill[2] + Fill_KC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        // [合計]
                        var tmpPa = Math.Ceiling(Good_All_Total / Fill_All_Total * 100);
                        if (double.IsNaN(tmpPa)) tmpPa = 0;
                        objSheet.Cells[iRow, 24].Value = string.Concat(tmpPa, "%");
                        objSheet.Cells[iRow, 37].Value = Fill_All_Total;  // 滿意度填答人數
                        // 丟到[整體]總計裡
                        DCBCKC_Sum_Good = DCBCKC_Sum_Good + Good_All_Total;//整體
                        DCBCKC_Sum_Fill = DCBCKC_Sum_Fill + Fill_All_Total;//整體
                    }

                    // 小型企業人力提升計畫
                    if (form.PlanTypeCode == "0" || form.PlanTypeCode == "2")
                    {
                        // 取出介接資料，條件：年度+該登入者底下的老師 (分署就是分署底下的老師清單，管理者就是全部老師)
                        Hashtable HtYear = new Hashtable { ["Year"] = form.Year };
                        var list_S_Class = dao.QueryForListAll<TransData_S_Class>("A3.Get_TransData_S_Class_TeachingHours", HtYear).Where(x => tmpTeachers_IDNO.Contains(x.TeacherID)).ToList();
                        //var list_S_Class = dao.GetRowList(new TransData_S_Class() { Year = form.Year.TONotNullString() }).Where(x => tmpTeachers_IDNO.Contains(x.TeacherID)).ToList();

                        // 授課時數
                        objSheet.Cells[iRow, 12].Value = list_S_Class.Where(x => x.JobAbilityCode == "DC").Sum(x => x.TeachHours) ?? 0;
                        objSheet.Cells[iRow, 13].Value = list_S_Class.Where(x => x.JobAbilityCode == "BC").Sum(x => x.TeachHours) ?? 0;
                        objSheet.Cells[iRow, 14].Value = list_S_Class.Where(x => x.JobAbilityCode == "KC").Sum(x => x.TeachHours) ?? 0;
                        objSheet.Cells[iRow, 15].Value = objSheet.Cells[iRow, 12].Value.TOInt32() + objSheet.Cells[iRow, 13].Value.TOInt32() + objSheet.Cells[iRow, 14].Value.TOInt32();
                        DCBCKC_Sum_Hour = DCBCKC_Sum_Hour + objSheet.Cells[iRow, 15].Value.TOInt32();
                        // 授課滿意度達70%以上對象表示滿意情形
                        double Good_DC_Sum = 0;  // DC 本身的加總，滿意的學員數
                        double Good_BC_Sum = 0;  // BC 本身的加總，滿意的學員數
                        double Good_KC_Sum = 0;  // KC 本身的加總，滿意的學員數
                        double Fill_DC_Sum = 0;  // DC 本身的加總，總學員數
                        double Fill_BC_Sum = 0;  // BC 本身的加總，總學員數
                        double Fill_KC_Sum = 0;  // KC 本身的加總，總學員數
                        double Good_All_Total = 0;  // [合計] 的加總用，滿意的學員數
                        double Fill_All_Total = 0;  // [合計] 的加總用，總學員數
                        foreach (var rowD in list_S_Class.Where(x => x.JobAbilityCode == "DC"))
                        {
                            // 列出該課程下總學員
                            Hashtable HtParms = new Hashtable
                            {
                                { "ClassDateS", rowD.ClassDateS },
                                { "TrainingTimeS", rowD.TrainingTimeS },
                                { "CourseUnitCode", rowD.CourseUnitCode },
                                { "TeacherID", rowD.TeacherID }
                            };
                            var list_S_Satisfy = dao.QueryForListAll<tbTransData_S_Satisfy>("A3.Get_tbTransData_S_Satisfy", HtParms).ToList();
                            //var list_S_Satisfy = dao.GetRowList(new TransData_S_Satisfy() { TrainID = rowD.TrainID, ClassID = rowD.ClassID, CourseUnitCode = rowD.CourseUnitCode, TeacherID = rowD.TeacherID, }).ToList(); 
                            // 列出滿意的學員
                            var list_S_Satisfy_Good = list_S_Satisfy.Where(x => (x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble()
                            + x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()) / 11 >= 4).ToList();
                            Good_DC_Sum = Good_DC_Sum + list_S_Satisfy_Good.ToCount();
                            Fill_DC_Sum = Fill_DC_Sum + list_S_Satisfy.ToCount();
                        }
                        var tmpPa_DC = Math.Ceiling(Good_DC_Sum / Fill_DC_Sum * 100);
                        if (double.IsNaN(tmpPa_DC)) tmpPa_DC = 0;
                        objSheet.Cells[iRow, 25].Value = string.Concat(tmpPa_DC, "%");
                        objSheet.Cells[iRow, 38].Value = Fill_DC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_DC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_DC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[3] = TheSum_DBK_Good[3] + Good_DC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[3] = TheSum_DBK_Fill[3] + Fill_DC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        foreach (var rowD in list_S_Class.Where(x => x.JobAbilityCode == "BC"))
                        {
                            // 列出該課程下總學員
                            Hashtable HtParms = new Hashtable
                            {
                                { "ClassDateS", rowD.ClassDateS },
                                { "TrainingTimeS", rowD.TrainingTimeS },
                                { "CourseUnitCode", rowD.CourseUnitCode },
                                { "TeacherID", rowD.TeacherID }
                            };
                            var list_S_Satisfy = dao.QueryForListAll<tbTransData_S_Satisfy>("A3.Get_tbTransData_S_Satisfy", HtParms).ToList();
                            //var list_S_Satisfy = dao.GetRowList(new TransData_S_Satisfy() { TrainID = rowD.TrainID, ClassID = rowD.ClassID, CourseUnitCode = rowD.CourseUnitCode, TeacherID = rowD.TeacherID, }).ToList();
                            // 列出滿意的學員
                            var list_S_Satisfy_Good = list_S_Satisfy.Where(x => (x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble()
                                + x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()) / 11 >= 4).ToList();
                            Good_BC_Sum = Good_BC_Sum + list_S_Satisfy_Good.ToCount();
                            Fill_BC_Sum = Fill_BC_Sum + list_S_Satisfy.ToCount();
                        }
                        var tmpPa_BC = Math.Ceiling(Good_BC_Sum / Fill_BC_Sum * 100);
                        if (double.IsNaN(tmpPa_BC)) tmpPa_BC = 0;
                        objSheet.Cells[iRow, 26].Value = string.Concat(tmpPa_BC, "%");
                        objSheet.Cells[iRow, 39].Value = Fill_BC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_BC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_BC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[4] = TheSum_DBK_Good[4] + Good_BC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[4] = TheSum_DBK_Fill[4] + Fill_BC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        foreach (var rowD in list_S_Class.Where(x => x.JobAbilityCode == "KC"))
                        {
                            // 列出該課程下總學員
                            Hashtable HtParms = new Hashtable
                            {
                                { "ClassDateS", rowD.ClassDateS },
                                { "TrainingTimeS", rowD.TrainingTimeS },
                                { "CourseUnitCode", rowD.CourseUnitCode },
                                { "TeacherID", rowD.TeacherID }
                            };
                            var list_S_Satisfy = dao.QueryForListAll<tbTransData_S_Satisfy>("A3.Get_tbTransData_S_Satisfy", HtParms).ToList();
                            //var list_S_Satisfy = dao.GetRowList(new TransData_S_Satisfy() { TrainID = rowD.TrainID, ClassID = rowD.ClassID, CourseUnitCode = rowD.CourseUnitCode, TeacherID = rowD.TeacherID, }).ToList(); 
                            // 列出滿意的學員
                            var list_S_Satisfy_Good = list_S_Satisfy.Where(x => (x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble()
                            + x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()) / 11 >= 4).ToList();
                            Good_KC_Sum = Good_KC_Sum + list_S_Satisfy_Good.ToCount();
                            Fill_KC_Sum = Fill_KC_Sum + list_S_Satisfy.ToCount();
                        }
                        var tmpPa_KC = Math.Ceiling(Good_KC_Sum / Fill_KC_Sum * 100);
                        if (double.IsNaN(tmpPa_KC)) tmpPa_KC = 0;
                        objSheet.Cells[iRow, 27].Value = string.Concat(tmpPa_KC, "%");
                        objSheet.Cells[iRow, 40].Value = Fill_KC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_KC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_KC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[5] = TheSum_DBK_Good[5] + Good_KC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[5] = TheSum_DBK_Fill[5] + Fill_KC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        // [合計]
                        var tmpPa = Math.Ceiling(Good_All_Total / Fill_All_Total * 100);
                        if (double.IsNaN(tmpPa)) tmpPa = 0;
                        objSheet.Cells[iRow, 28].Value = string.Concat(tmpPa, "%");
                        objSheet.Cells[iRow, 41].Value = Fill_All_Total;  // 滿意度填答人數
                        // 丟到[整體]總計裡
                        DCBCKC_Sum_Good = DCBCKC_Sum_Good + Good_All_Total;//整體
                        DCBCKC_Sum_Fill = DCBCKC_Sum_Fill + Fill_All_Total;//整體
                    }

                    // 企業人力資源提升計畫 
                    if (form.PlanTypeCode == "0" || form.PlanTypeCode == "3")
                    {
                        // 取出介接資料，條件：年度+該登入者底下的老師 (分署就是分署底下的老師清單，管理者就是全部老師)
                        Hashtable HtYear = new Hashtable { ["Year"] = form.Year };
                        var list_K_Class = dao.QueryForListAll<TransData_K_Class>("A3.Get_TransData_K_Class_TeachingHours", HtYear).Where(x => tmpTeachers_IDNO.Contains(x.TeacherID)).ToList();
                        //var list_S_Class = dao.GetRowList(new TransData_S_Class() { Year = form.Year.TONotNullString() }).Where(x => tmpTeachers_IDNO.Contains(x.TeacherID)).ToList();
                        var i_DC_o = list_K_Class.Where(x => x.JobAbilityCode == "DC").Sum(x => x.TeachHours) ?? 0;
                        var i_BC_o = list_K_Class.Where(x => x.JobAbilityCode == "BC").Sum(x => x.TeachHours) ?? 0;
                        var i_KC_o = list_K_Class.Where(x => x.JobAbilityCode == "KC").Sum(x => x.TeachHours) ?? 0;
                        var i_ALL_o = (i_DC_o + i_BC_o + i_KC_o);
                        var i_DC_x = list_K_Class.Where(x => x.JobAbilityCode == "DC").Sum(x => x.TeachHours_X) ?? 0;
                        var i_BC_x = list_K_Class.Where(x => x.JobAbilityCode == "BC").Sum(x => x.TeachHours_X) ?? 0;
                        var i_KC_x = list_K_Class.Where(x => x.JobAbilityCode == "KC").Sum(x => x.TeachHours_X) ?? 0;
                        var i_ALL_x = (i_DC_x + i_BC_x + i_KC_x);
                        var Ki_DC_x = (i_DC_x > 0) ? string.Concat("(", i_DC_x, ")") : "";
                        var Ki_BC_x = (i_BC_x > 0) ? string.Concat("(", i_BC_x, ")") : "";
                        var Ki_KC_x = (i_KC_x > 0) ? string.Concat("(", i_KC_x, ")") : "";
                        var Ki_ALL_x = i_ALL_x > 0 ? string.Concat("(", (i_ALL_o + i_ALL_x), ")") : "";
                        objSheet.Cells[iRow, 16].Value = GET_AB_VAL(i_DC_o, Ki_DC_x);
                        objSheet.Cells[iRow, 17].Value = GET_AB_VAL(i_BC_o, Ki_BC_x);
                        objSheet.Cells[iRow, 18].Value = GET_AB_VAL(i_KC_o, Ki_KC_x);
                        objSheet.Cells[iRow, 19].Value = GET_AB_VAL(i_ALL_o, Ki_ALL_x);
                        // 授課時數 Teaching hours
                        DCBCKC_Sum_Hour = DCBCKC_Sum_Hour + i_ALL_o;
                        DCBCKC_Sum_Hour_x = DCBCKC_Sum_Hour_x + i_ALL_x;
                        // 授課滿意度達70%以上對象表示滿意情形
                        double Good_DC_Sum = 0;  // DC 本身的加總，滿意的學員數
                        double Good_BC_Sum = 0;  // BC 本身的加總，滿意的學員數
                        double Good_KC_Sum = 0;  // KC 本身的加總，滿意的學員數
                        double Fill_DC_Sum = 0;  // DC 本身的加總，總學員數
                        double Fill_BC_Sum = 0;  // BC 本身的加總，總學員數
                        double Fill_KC_Sum = 0;  // KC 本身的加總，總學員數
                        double Good_All_Total = 0;  // [合計] 的加總用，滿意的學員數
                        double Fill_All_Total = 0;  // [合計] 的加總用，總學員數
                        foreach (var rowD in list_K_Class.Where(x => x.JobAbilityCode == "DC"))
                        {
                            // 列出該課程下總學員
                            Hashtable HtParms = new Hashtable
                            {
                                { "ClassDateS", rowD.ClassDateS }, { "TrainingTimeS", rowD.TrainingTimeS },
                                { "CourseUnitCode", rowD.CourseUnitCode }, { "TeacherID", rowD.TeacherID }
                            };
                            var list_K_Satisfy = dao.QueryForListAll<tbTransData_K_Satisfy>("A3.Get_tbTransData_K_Satisfy", HtParms).ToList();
                            //var list_S_Satisfy = dao.GetRowList(new TransData_S_Satisfy() { ClassDateS = rowD.ClassDateS, CourseUnitCode = rowD.CourseUnitCode, TeacherID = rowD.TeacherID }).ToList();
                            // 列出滿意的學員
                            var list_K_Satisfy_Good = list_K_Satisfy.Where(x => (x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble()
                            + x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()) / 11 >= 4).ToList();
                            Good_DC_Sum = Good_DC_Sum + list_K_Satisfy_Good.ToCount();
                            Fill_DC_Sum = Fill_DC_Sum + list_K_Satisfy.ToCount();
                        }
                        var tmpPa_DC = (Fill_DC_Sum > 0) ? Math.Ceiling(Good_DC_Sum / Fill_DC_Sum * 100) : 0;
                        if (double.IsNaN(tmpPa_DC)) tmpPa_DC = 0;
                        int iColK = 29;
                        objSheet.Cells[iRow, iColK].Value = string.Concat(tmpPa_DC, "%"); iColK = iColK + 13;
                        objSheet.Cells[iRow, iColK].Value = Fill_DC_Sum;  // 滿意度填答人數

                        Good_All_Total = Good_All_Total + Good_DC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_DC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[6] = TheSum_DBK_Good[6] + Good_DC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[6] = TheSum_DBK_Fill[6] + Fill_DC_Sum;  // 丟到[合計]總計裡 最下面一行的統計

                        foreach (var rowD in list_K_Class.Where(x => x.JobAbilityCode == "BC"))
                        {
                            // 列出該課程下總學員
                            Hashtable HtParms = new Hashtable
                            {
                                { "ClassDateS", rowD.ClassDateS },{ "TrainingTimeS", rowD.TrainingTimeS },
                                { "CourseUnitCode", rowD.CourseUnitCode },{ "TeacherID", rowD.TeacherID }
                            };
                            var list_K_Satisfy = dao.QueryForListAll<tbTransData_K_Satisfy>("A3.Get_tbTransData_K_Satisfy", HtParms).ToList();
                            // 列出滿意的學員
                            var list_K_Satisfy_Good = list_K_Satisfy.Where(x => (x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble()
                            + x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()) / 11 >= 4).ToList();
                            Good_BC_Sum = Good_BC_Sum + list_K_Satisfy_Good.ToCount();
                            Fill_BC_Sum = Fill_BC_Sum + list_K_Satisfy.ToCount();
                        }
                        var tmpPa_BC = (Fill_BC_Sum > 0) ? Math.Ceiling(Good_BC_Sum / Fill_BC_Sum * 100) : 0;
                        if (double.IsNaN(tmpPa_BC)) tmpPa_BC = 0;
                        iColK = 30;
                        objSheet.Cells[iRow, iColK].Value = string.Concat(tmpPa_BC, "%"); iColK = iColK + 13;
                        objSheet.Cells[iRow, iColK].Value = Fill_BC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_BC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_BC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[7] = TheSum_DBK_Good[7] + Good_BC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[7] = TheSum_DBK_Fill[7] + Fill_BC_Sum;  // 丟到[合計]總計裡 最下面一行的統計

                        foreach (var rowD in list_K_Class.Where(x => x.JobAbilityCode == "KC"))
                        {
                            // 列出該課程下總學員
                            Hashtable HtParms = new Hashtable
                            {
                                { "ClassDateS", rowD.ClassDateS },{ "TrainingTimeS", rowD.TrainingTimeS },
                                { "CourseUnitCode", rowD.CourseUnitCode },{ "TeacherID", rowD.TeacherID }
                            };
                            var list_K_Satisfy = dao.QueryForListAll<tbTransData_K_Satisfy>("A3.Get_tbTransData_K_Satisfy", HtParms).ToList();
                            // 列出滿意的學員
                            var list_K_Satisfy_Good = list_K_Satisfy.Where(x => (x.Q1.ToDouble() + x.Q2.ToDouble() + x.Q3.ToDouble() + x.Q4.ToDouble() + x.Q5.ToDouble()
                            + x.Q6.ToDouble() + x.Q7.ToDouble() + x.Q8.ToDouble() + x.Q9.ToDouble() + x.Q10.ToDouble() + x.Q11.ToDouble()) / 11 >= 4).ToList();
                            Good_KC_Sum = Good_KC_Sum + list_K_Satisfy_Good.ToCount();
                            Fill_KC_Sum = Fill_KC_Sum + list_K_Satisfy.ToCount();
                        }
                        var tmpPa_KC = (Fill_KC_Sum > 0) ? Math.Ceiling((Good_KC_Sum / Fill_KC_Sum) * 100) : 0;
                        if (double.IsNaN(tmpPa_KC)) tmpPa_KC = 0;
                        iColK = 31;
                        objSheet.Cells[iRow, iColK].Value = string.Concat(tmpPa_KC, "%"); iColK = iColK + 13;
                        objSheet.Cells[iRow, iColK].Value = Fill_KC_Sum;  // 滿意度填答人數
                        Good_All_Total = Good_All_Total + Good_KC_Sum;  // 丟到[合計]總計裡
                        Fill_All_Total = Fill_All_Total + Fill_KC_Sum;  // 丟到[合計]總計裡
                        TheSum_DBK_Good[8] = TheSum_DBK_Good[8] + Good_KC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        TheSum_DBK_Fill[8] = TheSum_DBK_Fill[8] + Fill_KC_Sum;  // 丟到[合計]總計裡 最下面一行的統計
                        // [合計]
                        var tmpPa = (Fill_All_Total > 0) ? Math.Ceiling((Good_All_Total / Fill_All_Total) * 100) : 0;
                        if (double.IsNaN(tmpPa)) tmpPa = 0;

                        iColK = 32;
                        objSheet.Cells[iRow, iColK].Value = string.Concat(tmpPa, "%"); iColK = iColK + 13;
                        objSheet.Cells[iRow, iColK].Value = Fill_All_Total;  // 滿意度填答人數
                        // 丟到[整體]總計裡
                        DCBCKC_Sum_Good = DCBCKC_Sum_Good + Good_All_Total;//整體
                        DCBCKC_Sum_Fill = DCBCKC_Sum_Fill + Fill_All_Total;//整體
                    }

                    var K_DCBCKC_Sum_Hour_x = (DCBCKC_Sum_Hour_x > 0) ? string.Concat("(", (DCBCKC_Sum_Hour + DCBCKC_Sum_Hour_x), ")") : "";
                    //授課總時數
                    objSheet.Cells[iRow, 20].Value = GET_AB_VAL(DCBCKC_Sum_Hour, K_DCBCKC_Sum_Hour_x);
                    var tmpPaAll = Math.Ceiling((DCBCKC_Sum_Good / DCBCKC_Sum_Fill) * 100);
                    if (double.IsNaN(tmpPaAll)) tmpPaAll = 0;
                    objSheet.Cells[iRow, 33].Value = string.Concat(tmpPaAll, "%");
                    objSheet.Cells[iRow, iLastCol].Value = DCBCKC_Sum_Fill;
                    // 計算統計
                    for (int i = 2; i <= iLastCol; i++)
                    {
                        // 因為匯出資料欄是2開始，但陣列從0，所以如下
                        TheSum[i - 2] = TheSum[i - 2] + GET_MY_VAL(objSheet.Cells[iRow, i].Value).TOInt32();
                        TheSum_X[i - 2] = TheSum_X[i - 2] + GET_MY_VAL_X(objSheet.Cells[iRow, i].Value).TOInt32();
                        TheSum_X2[i - 2] = TheSum_X2[i - 2] + GET_MY_VAL_X2(objSheet.Cells[iRow, i].Value).TOInt32();
                    }
                }

                //(系統管理者) 寫入統計
                if (sm.UserInfo.LoginCharacter == "4")
                {
                    for (int i = 2; i <= iLastCol; i++)
                    {
                        // 因為匯出資料欄是2開始，但陣列從0，所以如下
                        // 16.17.18: 企業人力資源提升計畫(非合計)
                        var iTheSum_X = (i >= 16 && i <= 18) ? TheSum_X[i - 2] : TheSum_X2[i - 2];
                        var COMBIV = GET_COMBI_VAL(TheSum[i - 2], iTheSum_X);
                        objSheet.Cells[10, i].Value = COMBIV;
                    }
                    // 0,1,2 = 大專
                    double tmp_Good = 0;
                    double tmp_Fill = 0;
                    for (int i = 0; i <= 2; i++)
                    {
                        var tmpPa = Math.Ceiling((TheSum_DBK_Good[i] / TheSum_DBK_Fill[i]) * 100);
                        if (double.IsNaN(tmpPa)) tmpPa = 0;
                        objSheet.Cells[10, 21 + i].Value = string.Concat(tmpPa, "%");
                        tmp_Good = tmp_Good + TheSum_DBK_Good[i];
                        tmp_Fill = tmp_Fill + TheSum_DBK_Fill[i];
                    }
                    var tmpPaAll1 = Math.Ceiling((tmp_Good / tmp_Fill) * 100);
                    if (double.IsNaN(tmpPaAll1)) tmpPaAll1 = 0;
                    objSheet.Cells[10, 24].Value = string.Concat(tmpPaAll1, "%");

                    // 3,4,5 = 小人提
                    tmp_Good = 0;
                    tmp_Fill = 0;
                    for (int i = 3; i <= 5; i++)
                    {
                        var tmpPa = Math.Ceiling((TheSum_DBK_Good[i] / TheSum_DBK_Fill[i]) * 100);
                        if (double.IsNaN(tmpPa)) tmpPa = 0;
                        objSheet.Cells[10, 25 + i - 3].Value = string.Concat(tmpPa, "%");
                        tmp_Good = tmp_Good + TheSum_DBK_Good[i];
                        tmp_Fill = tmp_Fill + TheSum_DBK_Fill[i];
                    }
                    var tmpPaAll2 = Math.Ceiling((tmp_Good / tmp_Fill) * 100);
                    if (double.IsNaN(tmpPaAll2)) tmpPaAll2 = 0;
                    objSheet.Cells[10, 28].Value = string.Concat(tmpPaAll2, "%");

                    // 6,7,8 = 大人提,企業人力資源提升計畫-授課滿意度達70 %以上對象表示滿意情形	
                    tmp_Good = 0;
                    tmp_Fill = 0;
                    for (int i = 6; i <= 8; i++)
                    {
                        var tmpPa = Math.Ceiling(TheSum_DBK_Good[i] / TheSum_DBK_Fill[i] * 100);
                        if (double.IsNaN(tmpPa)) tmpPa = 0;
                        objSheet.Cells[10, 26 + i - 3].Value = $"{tmpPa}%";
                        tmp_Good = tmp_Good + TheSum_DBK_Good[i];
                        tmp_Fill = tmp_Fill + TheSum_DBK_Fill[i];
                    }
                    var tmpPaAll2k = Math.Ceiling(tmp_Good / tmp_Fill * 100);
                    if (double.IsNaN(tmpPaAll2k)) tmpPaAll2k = 0;
                    objSheet.Cells[10, 32].Value = $"{tmpPaAll2k}%";

                    // 總計的總計
                    tmp_Good = 0;
                    tmp_Fill = 0;
                    var lastNum = 8;//5;
                    for (int i = 0; i <= lastNum; i++)
                    {
                        tmp_Good = tmp_Good + TheSum_DBK_Good[i];
                        tmp_Fill = tmp_Fill + TheSum_DBK_Fill[i];
                    }
                    var tmpPaAll3 = Math.Ceiling((tmp_Good / tmp_Fill) * 100);
                    if (double.IsNaN(tmpPaAll3)) tmpPaAll3 = 0;
                    objSheet.Cells[10, 33].Value = string.Concat(tmpPaAll3, "%");
                }
                // 後續處理
                objSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                objSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                objSheet.SelectedRange[2, 1, 4, iLastCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                objSheet.SelectedRange[2, 1, 4, iLastCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 242, 242));
                objSheet.Cells[1, 1, 10, iLastCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                objSheet.Cells[1, 1, 10, iLastCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                objSheet.Cells[1, 1, 10, iLastCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                objSheet.Cells[1, 1, 10, iLastCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                objSheet.Cells.Style.Font.Name = "新細明體";
                objSheet.Cells.Style.Font.Size = 12;
                objSheet.Cells.Style.WrapText = true;

                //備註
                var memomsg1 = "備註：若師資於大、小人提計畫有同日授課時數重複之情形(即同一堂外訓課程)，將於大人提計畫的職能類別欄位以（）呈現重複之時數。";
                objSheet.Cells[11, 1].Value = memomsg1;
                objSheet.SelectedRange[11, 1, 11, 20].Merge = true;
                objSheet.SelectedRange[11, 1, 11, 20].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                if (sm.UserInfo.LoginCharacter != "4")  // 分署匯出不需要總計與其他分署別
                {
                    objSheet.DeleteRow(10);
                    objSheet.DeleteRow(9);
                    objSheet.DeleteRow(8);
                    objSheet.DeleteRow(7);
                    objSheet.DeleteRow(6);
                }

                string s_FileName1 = string.Concat("師資授課情形統計表", "_", CommonsServices.GetDateTimeNow1());
                string s_FileName1_xlsx = string.Concat(s_FileName1, ".xlsx");
                // OK
                FuncLogAdd.Do(ModifyEvent.NONE, ModifyType.PRINT, ModifyState.SUCCESS, "A3/C502M", null, form, s_FileName1 + (form.ExportFormat == "0" ? ".xlsx" : ".ods"));
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



        /// <summary>組合大於0的v2字串</summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private object GET_COMBI_VAL(int v1, int v2)
        {
            if (v2 == 0 || v1 == v2) { return v1; }
            return string.Concat(v1, string.Concat("(", v2, ")"));
        }
        /// <summary>拆分'('為數字[0]</summary>
        /// <param name="objVAL"></param>
        /// <returns></returns>
        private object GET_MY_VAL(object objVAL)
        {
            char chVAL1 = '(';
            if (objVAL != null && Convert.ToString(objVAL).IndexOf(chVAL1) > -1)
            {
                return Convert.ToString(objVAL).ToSplit(chVAL1)[0];
            }
            return objVAL;
        }
        /// <summary>拆分'('為數字[1]</summary>
        /// <param name="objVAL"></param>
        /// <returns></returns>
        private object GET_MY_VAL_X(object objVAL)
        {
            char chVAL2 = ')';
            if (objVAL != null && Convert.ToString(objVAL).IndexOf(chVAL2) > -1)
            {
                objVAL = Convert.ToString(objVAL).ToSplit(chVAL2)[0];
                char chVAL1 = '(';
                if (Convert.ToString(objVAL).IndexOf(chVAL1) > -1)
                {
                    return Convert.ToString(objVAL).ToSplit(chVAL1)[1];
                }
            }
            return 0;
        }
        /// <summary>拆分'('為數字[1]X2</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private object GET_MY_VAL_X2(object objVAL)
        {
            char chVAL2 = ')';
            if (objVAL != null && Convert.ToString(objVAL).IndexOf(chVAL2) > -1)
            {
                objVAL = Convert.ToString(objVAL).ToSplit(chVAL2)[0];
                char chVAL1 = '(';
                if (Convert.ToString(objVAL).IndexOf(chVAL1) > -1)
                {
                    return Convert.ToString(objVAL).ToSplit(chVAL1)[1];
                }
            }
            return objVAL;
        }
        /// <summary>回傳數字或組合字</summary>
        /// <param name="VALo"></param>
        /// <param name="VALx"></param>
        /// <returns></returns>
        private object GET_AB_VAL(int VALo, string VALx)
        {
            if (string.IsNullOrEmpty(VALx)) { return VALo; }
            return string.Concat(VALo, VALx);
        }
    }
}
