using OfficeOpenXml;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Models;
using log4net;

namespace WKEFSERVICE.Services
{
    /// <summary>供匯出使用</summary>
    public class ExcelTitle
    {

        public static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //資料面匯出 - ReviewReport,Teacher //計算排程 -wkef1027, wkef1206

        /// <summary>關鍵就業力課程師資年度評核結果總表</summary>
        /// <param name="objSheet"></param>
        /// <param name="ht"></param>
        public void ShowExcelTitle1(ExcelWorksheet objSheet, Hashtable ht)
        {
            string fmYear = ht["fmYear"] != null ? ht["fmYear"].TONotNullString() : "";
            if (string.IsNullOrEmpty(fmYear)) return;

            var s_TitleNM1 = "關鍵就業力課程師資年度評核結果總表";
            var YEARS_ROC = (fmYear.TOInt32() - 1911).ToString();
            //var s_SheetNM = "年度評核結果總表";
            var s_TitleNM2 = $"{YEARS_ROC}年度{s_TitleNM1}";
            //var s_XLSX_NAME = string.Concat(s_SheetNM, ".xlsx");
            int iCol_MaxLength = 40;

            #region 年度評核結果總表 匯出程序 TITLE

            string tcol24_ao = "講師姓名,所屬轄區,產學類別,服務單位,職稱,服務單位屬性(工會、協會、管顧),最高學歷,學歷背景-學校1,學歷背景-科系1,生日,聯絡電話,聯絡手機,電子郵件,聯絡地址,共通核心\n職能老師,可授課職能類別";

            string tcol2_p = "回流訓";
            string tcol34_p = "是否出席,筆試測驗,結果";

            string tcol2_s = "授課情形";
            string tcol3_s = "補助大專校院辦理就業學程計畫,小型企業人力提升計畫,企業人力資源提升計畫";
            string tcol4_s = "DC,BC,KC,DC,BC,KC,DC,BC,KC";
            string tcol34_y1 = "授課總時數";
            string tcol34_y2 = "授課至少一個課程單元";

            string tcol2_z = "授課對象針對師資授課表示滿意之人數達70%以上"; //"授課滿意度達70%以上對象表示滿意情形";
            string tcol34_z = "補助大專校院辦理就業學程計畫,小型企業人力提升計畫,企業人力資源提升計畫,整體";

            string tcol234_a = "出席共識聯繫會議次數,是否繳交教學自我審核報告,評核各項指標結果,前一年度情形,評核結果,加入年份";

            //建立 Excel 物件
            //ExcelPackage objExcel = new ExcelPackage();
            //ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add(s_SheetNM);

            // 產生標題列 OOO 年度關鍵就業力課程師資年度評核結果總表																																	
            objSheet.Cells[1, 1].Value = s_TitleNM2;
            objSheet.SelectedRange[1, 1, 1, iCol_MaxLength].Merge = true;

            int iRow = 2;//目前停留行
            int iCol = 1;//目前停留欄
            int iToRow = -1;
            int iToCol = -1;
            int iRow2 = -1;
            int iCol2 = -1;
            foreach (string COLUMN_VAL in tcol24_ao.Split(','))
            {
                objSheet.Cells[iRow, iCol].Value = COLUMN_VAL;
                iToRow = iRow + 2; iToCol = iCol;
                objSheet.SelectedRange[iRow, iCol, iToRow, iToCol].Merge = true;
                iCol += 1;
            }
            objSheet.Cells[iRow, iCol].Value = tcol2_p;
            iToRow = iRow; iToCol = iCol + 2;
            objSheet.SelectedRange[iRow, iCol, iToRow, iToCol].Merge = true;

            iRow2 = iRow + 1; iCol2 = iCol;
            foreach (string COLUMN_VAL in tcol34_p.Split(','))
            {
                objSheet.Cells[iRow2, iCol2].Value = COLUMN_VAL;
                iToRow = iRow2 + 1; iToCol = iCol2;
                objSheet.SelectedRange[iRow2, iCol2, iToRow, iToCol].Merge = true;
                iCol2 += 1;
            }
            iCol = iToCol + 1;

            objSheet.Cells[iRow, iCol].Value = tcol2_s;
            iToRow = iRow; iToCol = iCol + tcol4_s.Split(',').Length + 1;
            objSheet.SelectedRange[iRow, iCol, iToRow, iToCol].Merge = true;

            iRow2 = iRow + 1; iCol2 = iCol;
            foreach (string COLUMN_VAL in tcol3_s.Split(','))
            {
                objSheet.Cells[iRow2, iCol2].Value = COLUMN_VAL;
                iToRow = iRow2; iToCol = iCol2 + 2;
                objSheet.SelectedRange[iRow2, iCol2, iToRow, iToCol].Merge = true;
                iCol2 = iToCol + 1;
            }
            iRow2 = iRow + 2; iCol2 = iCol;
            foreach (string COLUMN_VAL in tcol4_s.Split(','))
            {
                objSheet.Cells[iRow2, iCol2].Value = COLUMN_VAL;
                iCol2 += 1;
            }

            iRow2 = iRow + 1; iCol2 = iToCol + 1;
            objSheet.Cells[iRow2, iCol2].Value = tcol34_y1;
            iToRow = iRow2 + 1; iToCol = iCol2;
            objSheet.SelectedRange[iRow2, iCol2, iToRow, iToCol].Merge = true;

            iRow2 = iRow + 1; iCol2 = iCol2 + 1;
            objSheet.Cells[iRow2, iCol2].Value = tcol34_y2;
            iToRow = iRow2 + 1; iToCol = iCol2;
            objSheet.SelectedRange[iRow2, iCol2, iToRow, iToCol].Merge = true;

            iCol = iCol2 + 1;

            objSheet.Cells[iRow, iCol].Value = tcol2_z;
            iToRow = iRow; iToCol = iCol + 3;
            objSheet.SelectedRange[iRow, iCol, iToRow, iToCol].Merge = true;

            iRow2 = iRow + 1; iCol2 = iCol;
            foreach (string COLUMN_VAL in tcol34_z.Split(','))
            {
                objSheet.Cells[iRow2, iCol2].Value = COLUMN_VAL;
                iToRow = iRow2 + 1; iToCol = iCol2;
                objSheet.SelectedRange[iRow2, iCol2, iToRow, iToCol].Merge = true;
                iCol2 += 1;
            }
            iCol = iToCol + 1;

            foreach (string COLUMN_VAL in tcol234_a.Split(','))
            {
                objSheet.Cells[iRow, iCol].Value = COLUMN_VAL;
                iToRow = iRow + 2; iToCol = iCol;
                objSheet.SelectedRange[iRow, iCol, iToRow, iToCol].Merge = true;
                iCol += 1;
            }

            // 後續處理
            iToRow = 4;
            iToCol = iCol_MaxLength;
            objSheet.Row(1).Height = 20;
            objSheet.Row(2).Height = 20;
            objSheet.Row(3).Height = 60;
            objSheet.Row(4).Height = 20;

            objSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            objSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            objSheet.SelectedRange[2, 1, iToRow, iToCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            objSheet.SelectedRange[2, 1, iToRow, iToCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 242, 242));

            objSheet.Cells[1, 1, iToRow, iToCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            objSheet.Cells[1, 1, iToRow, iToCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            objSheet.Cells[1, 1, iToRow, iToCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            objSheet.Cells[1, 1, iToRow, iToCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            objSheet.Cells.Style.Font.Name = "新細明體";
            objSheet.Cells.Style.Font.Size = 10;
            objSheet.Cells.Style.WrapText = true;
            objSheet.SelectedRange[1, 1, 1, iCol_MaxLength].Style.Font.Size = 12;

            for (int iC1 = 1; iC1 <= iCol_MaxLength; iC1++)
            {
                objSheet.Column(iC1).Width = 12;
            }
            //講師姓名 
            objSheet.Column(1).Width = 16;
            //所屬轄區 
            objSheet.Column(2).Width = 20;
            //產學類別 
            objSheet.Column(3).Width = 16;
            //服務單位 
            objSheet.Column(4).Width = 36;
            //職稱 
            objSheet.Column(5).Width = 21;
            //服務單位屬性(工會、協會、管顧) 
            objSheet.Column(6).Width = 16;
            //最高學歷 objSheet.Column(7).Width = 10;
            //學歷背景-學校1 
            objSheet.Column(8).Width = 24;
            //學歷背景-科系1 
            objSheet.Column(9).Width = 28;
            //生日 objSheet.Column(10).Width = 10;
            //聯絡電話 
            objSheet.Column(11).Width = 16;
            //聯絡手機 
            objSheet.Column(12).Width = 16;
            //電子郵件 
            objSheet.Column(13).Width = 26;
            //聯絡地址 
            objSheet.Column(14).Width = 40;
            //共通核心職能老師
            objSheet.Column(15).Width = 16;
            //可授課職能類別 
            objSheet.Column(16).Width = 16;
            //加入年份 objSheet.Column(34).Width = 10;

            //回流訓-是否出席 objSheet.Column(16).Width = 10;
            //回流訓-筆試測驗 objSheet.Column(17).Width = 10;
            //回流訓-結果 objSheet.Column(18).Width = 10;

            //授課時數-補助大專校院辦理就業學程計畫-DC objSheet.Column(19).Width = 10;
            //授課時數-補助大專校院辦理就業學程計畫-BC objSheet.Column(20).Width = 10;
            //授課時數-補助大專校院辦理就業學程計畫-KC objSheet.Column(21).Width = 10;

            //授課時數-小型企業人力提升計畫-DC objSheet.Column(22).Width = 10;
            //授課時數-小型企業人力提升計畫-BC objSheet.Column(23).Width = 10;
            //授課時數-小型企業人力提升計畫-KC objSheet.Column(24).Width = 10;
            //授課時數-授課總時數 objSheet.Column(25).Width = 10;

            //授課滿意度達70%以上對象表示滿意情形-補助大專校院辦理就業學程計畫 objSheet.Column(26).Width = 10;
            //授課滿意度達70%以上對象表示滿意情形-小型企業人力提升計畫 objSheet.Column(27).Width = 10;
            //授課滿意度達70%以上對象表示滿意情形-整體 objSheet.Column(28).Width = 10;

            //出席共識聯繫會議次數 objSheet.Column(29).Width = 10;
            //是否繳交教學自我審核報告 objSheet.Column(30).Width = 10;
            //評核各項指標結果 objSheet.Column(31).Width = 10;
            //前一年度情形 objSheet.Column(32).Width = 10;
            //評核結果 objSheet.Column(33).Width = 10;
            #endregion
        }

        /// <summary>課程師資年度評核結果總表-資料填寫</summary>
        /// <param name="objSheet"></param>
        /// <param name="ht"></param>
        /// <param name="lstTeachers"></param>
        /// <param name="lstReviewReport"></param>
        public void SetExcelData(ExcelWorksheet objSheet, Hashtable ht, List<Teacher> lstTeachers, List<ReviewReport> lstReviewReport)
        {
            //資料面匯出 - ReviewReport //計算排程-wkef1206 
            //var fmYear = form.Year;
            //string sNowYear = DateTime.Now.Year.ToString();
            //string sNowYear_Roc = (DateTime.Now.Year - 1911).ToString();
            int ifmYear = DateTime.Now.Year; //目前系統年度
            string fmYear = ht["fmYear"] != null ? ht["fmYear"].TONotNullString() : ""; //查詢年度
            bool fg_OK1 = int.TryParse(fmYear, out ifmYear); //是否為數字(不論是否，皆會取得年度資訊)
            string sfmYear_Roc = (ifmYear - 1911).ToString(); //取得民國年度資訊(-1911)

            int iCol_MaxLength = -1;
            if (!int.TryParse(Convert.ToString(ht["Col_MaxLength"]), out iCol_MaxLength)) { return; }
            if (string.IsNullOrEmpty(fmYear) || lstTeachers == null) return;

            ShareCodeListModel sclm = new ShareCodeListModel();
            string s_TMP1 = null;
            int iRow = 4;
            foreach (var row1 in lstTeachers)
            {
                //2.確認，報表排除「下線」之老師，下線不顯示!!
                //下線：【Teacher】.Online = "N"
                if (string.IsNullOrEmpty(row1.UnitCode) || row1.UnitCode == "0" || row1.Online == "N") continue;

                iRow += 1;
                int iCol = 1;
                //講師姓名
                var TeacherName2 = row1.TeacherName;

                //lstReviewReport 搜尋資料依年度、身分證號、查無資料略過
                var row2 = lstReviewReport.Where(x => x.IDNO == row1.IDNO && x.Year == fmYear).FirstOrDefault();
                if (row2 != null)
                {
                    int DC_X = row2.THOURS_TRANSDATA_K_DC_X ?? 0;
                    int BC_X = row2.THOURS_TRANSDATA_K_BC_X ?? 0;
                    int KC_X = row2.THOURS_TRANSDATA_K_KC_X ?? 0;
                    if ((DC_X + BC_X + KC_X) > 0) { TeacherName2 += "(*)"; }
                }
                //講師姓名
                objSheet.Cells[iRow, iCol].Value = TeacherName2; iCol++;
                //所屬轄區
                //LOG.Debug(string.Concat("#sclm.UNIT_List.Count: ", sclm.UNIT_List.Count));
                //LOG.Debug(string.Concat("#row1.UnitCode: ", row1.UnitCode));
                //LOG.Debug(string.Concat("#string.IsNullOrEmpty(row1.UnitCode): ", string.IsNullOrEmpty(row1.UnitCode)));
                s_TMP1 = !string.IsNullOrEmpty(row1.UnitCode) ? sclm.UNIT_List.Where(x => x.Value == row1.UnitCode).FirstOrDefault().Text : "";
                objSheet.Cells[iRow, iCol].Value = s_TMP1; iCol++;
                //產學類別
                s_TMP1 = !string.IsNullOrEmpty(row1.IndustryAcademicType) ? sclm.IndustryAcademicType_List().Where(x => x.Value == row1.IndustryAcademicType).FirstOrDefault().Text : "";
                objSheet.Cells[iRow, iCol].Value = s_TMP1; iCol++;
                //服務單位
                objSheet.Cells[iRow, iCol].Value = row1.ServiceUnit1; iCol++;
                //職稱
                objSheet.Cells[iRow, iCol].Value = row1.JobTitle1; iCol++;
                //服務單位屬性(工會、協會、管顧)
                //form.ServiceUnitProperties = row1.ServiceUnitProperties;
                //s_TMP1 = form.ServiceUnitProperties_Name;
                s_TMP1 = sclm.ServiceUnitPropertiesName(row1.ServiceUnitProperties);
                objSheet.Cells[iRow, iCol].Value = s_TMP1; iCol++;
                //最高學歷
                s_TMP1 = !string.IsNullOrEmpty(row1.EduLevelHighest) ? sclm.EduLevel_List.Where(x => x.Value == row1.EduLevelHighest).FirstOrDefault().Text : "";
                objSheet.Cells[iRow, iCol].Value = s_TMP1; iCol++;
                //學歷背景-學校1
                objSheet.Cells[iRow, iCol].Value = row1.EduSchool1; iCol++;
                //學歷背景-科系1
                objSheet.Cells[iRow, iCol].Value = row1.EduDept1; iCol++;
                //生日
                DateTime tmpBirth = new DateTime();
                s_TMP1 = DateTime.TryParse(row1.Birthday, out tmpBirth) ? tmpBirth.ToString("yyyy/MM/dd") : "";
                //DateTime tmp = HelperUtil.TransToDateTime(row1.Birthday, "/") ?? DateTime.MinValue;/HelperUtil.DateTimeToString(tmp, "/");//YYYMMDD回傳給系統
                objSheet.Cells[iRow, iCol].Value = s_TMP1; iCol++;
                //聯絡電話
                objSheet.Cells[iRow, iCol].Value = row1.Tel; iCol++;
                //聯絡手機
                objSheet.Cells[iRow, iCol].Value = row1.Phone; iCol++;
                //電子郵件
                objSheet.Cells[iRow, iCol].Value = row1.Email; iCol++;
                //聯絡地址
                objSheet.Cells[iRow, iCol].Value = row1.Address; iCol++;
                //共通核心 職能老師
                objSheet.Cells[iRow, iCol].Value = (row1.PublicCore == "Y" ? "是" : ""); iCol++;
                //可授課職能類別
                s_TMP1 = "";
                s_TMP1 += row1.TeachJobAbilityDC == "Y" ? $"{(s_TMP1 != "" ? "," : "")}DC" : "";
                s_TMP1 += row1.TeachJobAbilityBC == "Y" ? $"{(s_TMP1 != "" ? "," : "")}BC" : "";
                s_TMP1 += row1.TeachJobAbilityKC == "Y" ? $"{(s_TMP1 != "" ? "," : "")}KC" : "";
                objSheet.Cells[iRow, iCol].Value = s_TMP1; iCol++;

                //1.針對「評核各項指標結果」、「評核結果」 (請參附件黃底的欄位)增加判斷邏輯
                // 判斷【加入年份】欄位，若【加入年份】為當年度(查詢年度)，則不須判斷當年度加入之師資各項評核指標，
                // 直接於「評核各項指標結果」顯示「無」、評核結果為「維持師資資格」
                // 範例：今年為112年，則【加入年份】為112年者，系統直接匯出顯示 【評核各項指標結果】：無、評核結果：維持師資資格
                bool fgSameYear1 = (sfmYear_Roc == row1.JoinYear);

                //var rowIDNO = row1.IDNO;
                //lstReviewReport 搜尋資料依年度、身分證號、查無資料略過
                //var row2 = lstReviewReport.Where(x => x.IDNO == row1.IDNO && x.Year == fmYear).FirstOrDefault();
                if (row2 == null) continue;

                //回流訓-是否出席
                objSheet.Cells[iRow, iCol].Value = row2.MT1_ATTENDANCE == "Y" ? "是" : "否"; iCol++;
                //回流訓-筆試測驗
                objSheet.Cells[iRow, iCol].Value = row2.MT1_WRITTEN_EXAM == "Y" ? "通過" : "不通過"; iCol++;
                //回流訓-結果
                objSheet.Cells[iRow, iCol].Value = row2.MT1_RESULTS == "Y" ? "完成" : "未完成"; iCol++;

                int? hour_D_DC = row2.THOURS_TRANSDATA_D_DC;
                int? hour_D_BC = row2.THOURS_TRANSDATA_D_BC;
                int? hour_D_KC = row2.THOURS_TRANSDATA_D_KC;
                int? hour_S_DC = row2.THOURS_TRANSDATA_S_DC;
                int? hour_S_BC = row2.THOURS_TRANSDATA_S_BC;
                int? hour_S_KC = row2.THOURS_TRANSDATA_S_KC;
                int? hour_K_DC = row2.THOURS_TRANSDATA_K_DC;
                int? hour_K_BC = row2.THOURS_TRANSDATA_K_BC;
                int? hour_K_KC = row2.THOURS_TRANSDATA_K_KC;
                int? hour_K_DC_X = row2.THOURS_TRANSDATA_K_DC_X;
                int? hour_K_BC_X = row2.THOURS_TRANSDATA_K_BC_X;
                int? hour_K_KC_X = row2.THOURS_TRANSDATA_K_KC_X;
                int hour_K_ALL_X = hour_K_DC_X.TOInt32() + hour_K_BC_X.TOInt32() + hour_K_KC_X.TOInt32();
                int hour_D = row2.LEASTONEUNIT_D.HasValue ? row2.LEASTONEUNIT_D.Value : 0;
                int hour_S = row2.LEASTONEUNIT_S.HasValue ? row2.LEASTONEUNIT_S.Value : 0;
                int hour_K = row2.LEASTONEUNIT_K.HasValue ? row2.LEASTONEUNIT_K.Value : 0;
                int? hour_D_TOTAL = row2.THOURS_TRANSDATA_D_TOTAL;
                int? hour_S_TOTAL = row2.THOURS_TRANSDATA_S_TOTAL;
                int? hour_K_TOTAL = row2.THOURS_TRANSDATA_K_TOTAL;
                var s_hour_K_DC_GX = hour_K_DC.HasValue ? hour_K_DC.Value.ToString() : "";
                var s_hour_K_BC_GX = hour_K_BC.HasValue ? hour_K_BC.Value.ToString() : "";  //row2.THOURS_TRANSDATA_K_BC_X;
                var s_hour_K_KC_GX = hour_K_KC.HasValue ? hour_K_KC.Value.ToString() : "";  //row2.THOURS_TRANSDATA_K_KC_X;
                s_hour_K_DC_GX += (hour_K_DC_X.HasValue && hour_K_DC_X.Value > 0) ? $"({hour_K_DC_X.Value})" : "";
                s_hour_K_BC_GX += (hour_K_BC_X.HasValue && hour_K_BC_X.Value > 0) ? $"({hour_K_BC_X.Value})" : "";  //row2.THOURS_TRANSDATA_K_BC_X;
                s_hour_K_KC_GX += (hour_K_KC_X.HasValue && hour_K_KC_X.Value > 0) ? $"({hour_K_KC_X.Value})" : "";  //row2.THOURS_TRANSDATA_K_KC_X;

                //授課時數-補助大專校院辦理就業學程計畫-DC
                objSheet.Cells[iRow, iCol].Value = hour_D_DC; iCol++;
                //授課時數-補助大專校院辦理就業學程計畫-BC
                objSheet.Cells[iRow, iCol].Value = hour_D_BC; iCol++;
                //授課時數-補助大專校院辦理就業學程計畫-KC
                objSheet.Cells[iRow, iCol].Value = hour_D_KC; iCol++;
                //授課時數-小型企業人力提升計畫-DC
                objSheet.Cells[iRow, iCol].Value = hour_S_DC; iCol++;
                //授課時數-小型企業人力提升計畫-BC
                objSheet.Cells[iRow, iCol].Value = hour_S_BC; iCol++;
                //授課時數-小型企業人力提升計畫-KC
                objSheet.Cells[iRow, iCol].Value = hour_S_KC; iCol++;
                //授課時數-企業人力資源提升計畫-DC
                objSheet.Cells[iRow, iCol].Value = s_hour_K_DC_GX; iCol++;
                //授課時數-企業人力資源提升計畫-BC
                objSheet.Cells[iRow, iCol].Value = s_hour_K_BC_GX; iCol++;
                //授課時數-企業人力資源提升計畫-KC
                objSheet.Cells[iRow, iCol].Value = s_hour_K_KC_GX; iCol++;

                //授課時數-授課總時數
                int? hour_DSK_TOTAL = (hour_D_TOTAL.HasValue ? hour_D_TOTAL.Value : 0) + (hour_S_TOTAL.HasValue ? hour_S_TOTAL.Value : 0) + (hour_K_TOTAL.HasValue ? hour_K_TOTAL.Value : 0);
                if (!hour_D_TOTAL.HasValue && !hour_S_TOTAL.HasValue && !hour_K_TOTAL.HasValue) hour_DSK_TOTAL = null;
                //授課時數-授課總時數
                var THOURS_TRANSDATA_DSK_TOTAL = row2.THOURS_TRANSDATA_D_TOTAL.TOInt32() + row2.THOURS_TRANSDATA_S_TOTAL.TOInt32() + row2.THOURS_TRANSDATA_K_TOTAL.TOInt32();
                //授課時數-授課總時數(師資於大、小人提計畫有同日授課時數重複之情形)
                int hour_TOTAL_ALL_X = (hour_K_ALL_X > 0) ? (THOURS_TRANSDATA_DSK_TOTAL + hour_K_ALL_X) : 0;
                var V_hour_TOTAL_X = (hour_TOTAL_ALL_X > 0) ? $"({hour_TOTAL_ALL_X})" : null;
                objSheet.Cells[iRow, iCol].Value = GET_AB_VAL(hour_DSK_TOTAL, V_hour_TOTAL_X); iCol++;

                //授課至少一個課程單元
                bool fb_LEASTONEUNIT = ((hour_D + hour_S + hour_K) > 0);
                objSheet.Cells[iRow, iCol].Value = fb_LEASTONEUNIT ? "是" : ""; iCol++;

                var tmpST_D = Math.Ceiling(row2.SATISFY_TRANSDATA_D.HasValue ? row2.SATISFY_TRANSDATA_D.Value : 0);
                var tmpST_S = Math.Ceiling(row2.SATISFY_TRANSDATA_S.HasValue ? row2.SATISFY_TRANSDATA_S.Value : 0);
                var tmpST_K = Math.Ceiling(row2.SATISFY_TRANSDATA_K.HasValue ? row2.SATISFY_TRANSDATA_K.Value : 0);

                //decimal? tmpST_DS = null;
                //if (tmpST_D > 0 && tmpST_S > 0)
                //{
                //    tmpST_DS = Math.Ceiling((tmpST_D + tmpST_S) / 2);
                //}
                //else if (tmpST_D > 0)
                //{
                //    tmpST_DS = tmpST_D;
                //}
                //else if (tmpST_S > 0)
                //{
                //    tmpST_DS = tmpST_S;
                //}

                var strST_D = tmpST_D > 0 ? $"{tmpST_D}%" : "";
                var strST_S = tmpST_S > 0 ? $"{tmpST_S}%" : "";
                var strST_K = tmpST_K > 0 ? $"{tmpST_K}%" : "";

                //授課滿意度達70%以上對象表示滿意情形-補助大專校院辦理就業學程計畫 
                objSheet.Cells[iRow, iCol].Value = strST_D; iCol++;
                //授課滿意度達70%以上對象表示滿意情形-小型企業人力提升計畫
                objSheet.Cells[iRow, iCol].Value = strST_S; iCol++;
                //授課滿意度達70%以上對象表示滿意情形-企業人力資源提升計畫
                objSheet.Cells[iRow, iCol].Value = strST_K; iCol++;

                //授課滿意度達70%以上對象表示滿意情形-整體
                var tmpST_DS = Math.Ceiling(row2.SATISFY_TRANSDATA_DS ?? 0);
                var strST_DS = tmpST_DS > 0 ? $"{tmpST_DS}%" : "";
                objSheet.Cells[iRow, iCol].Value = strST_DS; iCol++;

                //授課滿意度達70%以上對象表示滿意情形-整體
                //var tmpST_DSK = Math.Ceiling(row2.SATISFY_TRANSDATA_DSK ?? 0);
                //var strST_DSK = tmpST_DSK > 0 ? $"{tmpST_DSK}%" : "";
                //objSheet.Cells[iRow, iCol].Value = strST_DSK; iCol++;

                //1、有出席回流訓並通過筆試,//2、授課總時數達36小時,//3、整體滿意度達70 % 以上,//4、出席共識會議2次(含)以上,//5、有繳交自我審核報告,

                //出席共識聯繫會議次數
                objSheet.Cells[iRow, iCol].Value = row2.MEETING_TIMES; iCol++;
                //是否繳交教學自我審核報告
                objSheet.Cells[iRow, iCol].Value = row2.REPORTREC_YN == "Y" ? "是" : "否"; iCol++;

                //評核各項指標結果
                objSheet.Cells[iRow, iCol].Value = fgSameYear1 ? "無" : row2.POINT_RESULTS == "Y" ? "已達門檻" : "未達門檻"; iCol++;
                //前一年度情形
                objSheet.Cells[iRow, iCol].Value = row2.PYEARS_STATUS == "Y" ? "已達門檻" : row2.PYEARS_STATUS == "N" ? "未達門檻" : "無"; iCol++;
                //評核結果
                s_TMP1 = fgSameYear1 ? "維持師資資格" : row2.REVIEW_RESULTS == "01" ? "列入觀察名單" : row2.REVIEW_RESULTS == "02" ? "維持師資資格" : row2.REVIEW_RESULTS == "03" ? "不予納入師資名單" : "";
                objSheet.Cells[iRow, iCol].Value = s_TMP1; iCol++;
                //加入年份
                objSheet.Cells[iRow, iCol].Value = row1.JoinYear; iCol++;
            }
            iRow += 1;
            //備註
            var memomsg1 = "備註：*表示師資於大、小人提計畫有同日授課時數重複之情形(即同一堂外訓課程)，將於大人提計畫的職能類別欄位以（）呈現重複之時數。";
            objSheet.Cells[iRow, 1].Value = memomsg1;
            objSheet.SelectedRange[iRow, 1, iRow, iCol_MaxLength].Merge = true;
            objSheet.SelectedRange[iRow, 1, iRow, iCol_MaxLength].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            //資料後續處理
            objSheet.Cells[1, 1, iRow, iCol_MaxLength].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            objSheet.Cells[1, 1, iRow, iCol_MaxLength].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            objSheet.Cells[1, 1, iRow, iCol_MaxLength].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            objSheet.Cells[1, 1, iRow, iCol_MaxLength].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

        }

        /// <summary>拆分'('為數字</summary>
        /// <param name="objVAL"></param>
        /// <returns></returns>
        private object GET_MY_VAL(object objVAL)
        {
            char chVAL1 = '(';
            if (objVAL != null && $"{objVAL}".IndexOf(chVAL1) > -1)
            {
                return $"{objVAL}".ToSplit(chVAL1)[0];
            }
            return objVAL;
        }
        /// <summary>回傳NULL.數字或組合字</summary>
        /// <param name="VALo"></param>
        /// <param name="VALx"></param>
        /// <returns></returns>
        private object GET_AB_VAL(int? VALo, string VALx)
        {
            if (string.IsNullOrEmpty(VALx) || !VALo.HasValue) { return VALo; }
            return $"{VALo}{VALx}";
        }
    }
}
