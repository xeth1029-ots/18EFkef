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
using System.Web;
using System.IO;
using log4net;
using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C105MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log

        [HttpGet]
        public ActionResult Index()
        {
            C105MViewModel model = new C105MViewModel();
            model.Form.FilterTab = "1";
            model.Form.ExpYear_Tab1 = $"{DateTime.Now.AddMonths(-1).Year}";
            model.Form.ExpMonth_Tab1 = $"{DateTime.Now.AddMonths(-1).Month}";
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C105MFormModel form)
        {
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                if (form.FilterTab == "1") form.Grid = dao.QueryC105M_1(form);
                if (form.FilterTab == "2") form.Grid = dao.QueryC105M_2(form);
                if (form.FilterTab == "3") form.Grid = dao.QueryC105M_3(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        /// <summary>
        /// 匯出每月登入異常日誌
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IdxExport(C105MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            ActionResult rtn = View("Index", form);
            if (form.ExpYear_Tab1.TONotNullString() == "") { sm.LastErrorMessage = "請選擇匯出年度！"; return rtn; }
            if (form.ExpMonth_Tab1.TONotNullString() == "") { sm.LastErrorMessage = "請選擇匯出月度！"; return rtn; }

            AMDAO dao = new AMDAO();
            form.Grid2 = dao.QueryC105M_1B(form);
            if (form.Grid2.ToCount() <= 0) { sm.LastErrorMessage = "查無資料！"; return rtn; }

            string strSheetNM = "匯出每月登入異常日誌";

            // 匯出程序
            try
            {
                // 建立 Excel 物件
                ExcelPackage objExcel = new ExcelPackage();
                ExcelWorksheet objSheet = objExcel.Workbook.Worksheets.Add(strSheetNM);
                // 一些暫存變數
                string[] DisplayCols = { "帳號", "姓名", "紀錄時間", "ADDIP", "使用瀏覽器", "登入訊息", "訊息", "單位", "角色" };
                string[] DisplayFields = { "username", "usr_name", "createtime", "ip", "useragent", "logtype_Name", "message", "unit_name", "grp_name" };
                // 產生 標題列
                string strTitle1 = $"{strSheetNM}{(form.ExpYear_Tab1.TOInt32() - 1911)}年{form.ExpMonth_Tab1}月";
                objSheet.Cells[1, 1].Value = strTitle1;
                objSheet.Cells[2, 1].Value = "帳號：";
                objSheet.Cells[2, 2].Value = sm.UserInfo.User.USERNAME;
                objSheet.Cells[2, 3].Value = "單位：";
                objSheet.Cells[2, 4].Value = sm.UserInfo.User.UNIT_NAME;
                // 標題
                int titRow = 3;
                for (int i = 1; i < DisplayCols.Length + 1; i++)
                {
                    objSheet.Cells[titRow, i].Value = DisplayCols[i - 1];
                    objSheet.Cells[titRow, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[titRow, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[titRow, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    objSheet.Cells[titRow, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                // 開始塞資料
                int Row = 4;
                foreach (var row in form.Grid2)
                {
                    for (int i = 1; i < DisplayFields.Length + 1; i++)
                    {
                        objSheet.Cells[Row, i].Value = row.GetPiValue(DisplayFields[i - 1]).TONotNullString();
                        objSheet.Cells[Row, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        objSheet.Cells[Row, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        objSheet.Cells[Row, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        objSheet.Cells[Row, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        objSheet.Cells[Row, i].AutoFitColumns(12, 120);
                        //objSheet.Column(objSheet.Cells[Row, i].Start.Column).BestFit = true;
                    }
                    Row++;
                }
                // 後續處理
                objSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                objSheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                objSheet.SelectedRange[1, 1, 1, DisplayCols.Length].Merge = true;
                //objSheet.SelectedRange[2, 4, 2, DisplayCols.Length].Merge = true;
                objSheet.SelectedRange[titRow, 1, titRow, DisplayCols.Length].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                objSheet.SelectedRange[titRow, 1, titRow, DisplayCols.Length].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 242, 242));
                //for (int i = 1; i <= DisplayCols.Length; i++) objSheet.Column(i).Width = 14;
                objSheet.Column(1).Width = 13;
                objSheet.Column(2).Width = 13;
                objSheet.Column(3).Width = 22;
                objSheet.Column(4).Width = 22;
                objSheet.Row(titRow).Height = 45;
                objSheet.Cells.Style.Font.Name = "新細明體";
                objSheet.Cells.Style.Font.Size = 12;
                objSheet.Cells.Style.WrapText = true;

                string s_FileName1 = $"{strTitle1}_{CommonsServices.GetDateTimeNow1()}";
                string s_FileName1_xlsx = $"{s_FileName1}.xlsx";
                FuncLogAdd.Do(ModifyEvent.NONE, ModifyType.PRINT, ModifyState.SUCCESS, "AM/C105M", null, form, s_FileName1_xlsx);
                return File(objExcel.GetAsByteArray(), HelperUtil.XLSXContentType, s_FileName1_xlsx);
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                sm.LastErrorMessage = $"發生錯誤！,({ex.Message})";
            }
            return rtn;
        }


    }
}