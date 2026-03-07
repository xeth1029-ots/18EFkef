using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Services;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Models;
using System.Web.Mvc;
using Turbo.Commons;
using System.Linq;
using System;


namespace WKEFSERVICE.Areas.AM.Models
{
    public class C105MViewModel
    {
        public C105MViewModel() { this.Form = new C105MFormModel(); }

        public C105MFormModel Form { get; set; }
    }

    public class C105MFormModel : PagingResultsViewModel
    {
        public C105MFormModel()
        {
            this.CreateTime_D1_Tab1 = "23";
            this.CreateTime_D2_Tab1 = "59";
            this.CreateTime_D1_Tab2 = "23";
            this.CreateTime_D2_Tab2 = "59";
            this.CreateTime_D1_Tab3 = "23";
            this.CreateTime_D2_Tab3 = "59";
        }

        public string FilterTab { get; set; }

        [Display(Name = "類型")]
        public string FilterType_Tab1 { get; set; }

        public IList<SelectListItem> FilterType_Tab1_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.C105MFilterType_Tab1_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "類型")]
        public string FilterType_Tab2 { get; set; }

        public IList<SelectListItem> FilterType_Tab2_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.C105MFilterType_Tab2_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "操作項目")]
        public string FilterType_Tab3 { get; set; }

        public IList<SelectListItem> FilterType_Tab3_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.C105MFilterType_Tab3_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "功能項目")]
        public string FilterFunc_Tab3 { get; set; }

        public IList<SelectListItem> FilterFunc_Tab3_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.C105MFilterFunc_Tab3_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "帳號")]
        public string FilterAccount_Tab1 { get; set; }

        [Display(Name = "帳號")]
        public string FilterAccount_Tab2 { get; set; }

        [Display(Name = "帳號")]
        public string FilterAccount_Tab3 { get; set; }

        [Display(Name = "匯出年月")]
        public string ExpYearMonth_Tab1 { get; set; }

        [Display(Name = "匯出年")]
        public string ExpYear_Tab1 { get; set; }

        [Display(Name = "匯出月")]
        public string ExpMonth_Tab1 { get; set; }
        /// <summary>
        /// 年度清單
        /// </summary>
        public IList<SelectListItem> Year_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var list = model.ReportRecordYears_List();
                list.Insert(0, new SelectListItem() { Text = "未選擇", Value = "" });
                return list;
            }
        }

        /// <summary>
        /// 月份清單
        /// </summary>
        public IList<SelectListItem> Month_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var list = model.ReportRecordMonth_List();
                list.Insert(0, new SelectListItem() { Text = "未選擇", Value = "" });
                return list;
            }
        }

        public string CreateTime_A_Tab1 { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string CreateTime_A_Tab1_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(CreateTime_A_Tab1, "/"), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                CreateTime_A_Tab1 = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string CreateTime_B_Tab1 { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string CreateTime_B_Tab1_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(CreateTime_B_Tab1, "/"), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                CreateTime_B_Tab1 = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string CreateTime_C1_Tab1 { get; set; }

        public IList<SelectListItem> CreateTime_C1_Tab1_list { get { return new ShareCodeListModel().TimeHour_List(); } }

        public string CreateTime_C2_Tab1 { get; set; }

        public IList<SelectListItem> CreateTime_C2_Tab1_list { get { return new ShareCodeListModel().TimeMin_List(); } }

        public string CreateTime_D1_Tab1 { get; set; }

        public IList<SelectListItem> CreateTime_D1_Tab1_list { get { return new ShareCodeListModel().TimeHour_List(); } }

        public string CreateTime_D2_Tab1 { get; set; }

        public IList<SelectListItem> CreateTime_D2_Tab1_list { get { return new ShareCodeListModel().TimeMin_List(); } }

        public string CreateTime_C_Tab1 { get { return this.CreateTime_C1_Tab1 + ":" + CreateTime_C2_Tab1; } }

        public string CreateTime_D_Tab1 { get { return this.CreateTime_D1_Tab1 + ":" + CreateTime_D2_Tab1; } }

        public string CreateTime_A_Tab2 { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string CreateTime_A_Tab2_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(CreateTime_A_Tab2, "/"), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                CreateTime_A_Tab2 = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string CreateTime_B_Tab2 { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string CreateTime_B_Tab2_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(CreateTime_B_Tab2, "/"), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                CreateTime_B_Tab2 = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string CreateTime_C1_Tab2 { get; set; }

        public IList<SelectListItem> CreateTime_C1_Tab2_list { get { return new ShareCodeListModel().TimeHour_List(); } }

        public string CreateTime_C2_Tab2 { get; set; }

        public IList<SelectListItem> CreateTime_C2_Tab2_list { get { return new ShareCodeListModel().TimeMin_List(); } }

        public string CreateTime_D1_Tab2 { get; set; }

        public IList<SelectListItem> CreateTime_D1_Tab2_list { get { return new ShareCodeListModel().TimeHour_List(); } }

        public string CreateTime_D2_Tab2 { get; set; }

        public IList<SelectListItem> CreateTime_D2_Tab2_list { get { return new ShareCodeListModel().TimeMin_List(); } }

        public string CreateTime_C_Tab2 { get { return this.CreateTime_C1_Tab2 + ":" + CreateTime_C2_Tab2; } }

        public string CreateTime_D_Tab2 { get { return this.CreateTime_D1_Tab2 + ":" + CreateTime_D2_Tab2; } }

        public string CreateTime_A_Tab3 { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string CreateTime_A_Tab3_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(CreateTime_A_Tab3, "/"), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                CreateTime_A_Tab3 = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string CreateTime_B_Tab3 { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string CreateTime_B_Tab3_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(CreateTime_B_Tab3, "/"), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                CreateTime_B_Tab3 = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string CreateTime_C1_Tab3 { get; set; }

        public IList<SelectListItem> CreateTime_C1_Tab3_list { get { return new ShareCodeListModel().TimeHour_List(); } }

        public string CreateTime_C2_Tab3 { get; set; }

        public IList<SelectListItem> CreateTime_C2_Tab3_list { get { return new ShareCodeListModel().TimeMin_List(); } }

        public string CreateTime_D1_Tab3 { get; set; }

        public IList<SelectListItem> CreateTime_D1_Tab3_list { get { return new ShareCodeListModel().TimeHour_List(); } }

        public string CreateTime_D2_Tab3 { get; set; }

        public IList<SelectListItem> CreateTime_D2_Tab3_list { get { return new ShareCodeListModel().TimeMin_List(); } }

        public string CreateTime_C_Tab3 { get { return this.CreateTime_C1_Tab3 + ":" + CreateTime_C2_Tab3; } }

        public string CreateTime_D_Tab3 { get { return this.CreateTime_D1_Tab3 + ":" + CreateTime_D2_Tab3; } }

        public IList<C105MGridModel> Grid { get; set; }
        public IList<C105MGrid2Model> Grid2 { get; set; }

    }

    public class C105MGridModel : loginlog
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public long? id { get; set; }

        /// <summary>
        /// 這個是存 使用者帳號
        /// </summary>
        [NotDBField]
        public string username { get; set; }

        /// <summary>
        /// 這個是存 使用者名稱
        /// </summary>
        [NotDBField]
        public string USER_NAME { get; set; }

        [NotDBField]
        public string logtype { get; set; }

        [NotDBField]
        public string logtype_Name
        {
            get
            {
                string result = this.logtype.TONotNullString();
                result = result.Replace("LOGINFAIL", "登入失敗");
                result = result.Replace("LOGIN", "登入");
                result = result.Replace("LOGOUT", "登出");
                result = result.Replace("CREATE", "新增");
                result = result.Replace("UPDATE", "修改");
                result = result.Replace("DELETE", "刪除");
                result = result.Replace("UPLOAD", "上傳");
                result = result.Replace("DOWNLOAD", "下載");
                result = result.Replace("PRINT", "匯出報表");
                if (result.Contains("BEFORE")) { result = result.Replace("BEFORE", ""); result = result + "前"; }
                if (result.Contains("AFTER")) { result = result.Replace("AFTER", ""); result = result + "後"; }
                return result;
            }
        }

        [NotDBField]
        public string createtime { get; set; }

        [NotDBField]
        public string createtime_Name
        {
            get
            {
                if (createtime.TONotNullString() == "") return "";
                string[] tmpList = createtime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                if (tmpDate.IndexOf("/") > 0) tmpDate = tmpDate.Replace("/", "-");
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }

        [NotDBField]
        public string ip { get; set; }

        [NotDBField]
        public string message { get; set; }

        [NotDBField]
        public string useragent { get; set; }

        [NotDBField]
        public string logfunc { get; set; }

        [NotDBField]
        public string result { get; set; }

        [NotDBField]
        public string result_Name
        {
            get
            {
                if (this.result.TONotNullString() == "SUCCESS") return "成功";
                if (this.result.TONotNullString() == "FAIL") return "失敗";
                return this.result.TONotNullString();
            }
        }

        [NotDBField]
        public string filename { get; set; }

        [NotDBField]
        public string arg { get; set; }

        [NotDBField]
        public string arg_Name
        {
            get
            {
                if (this.arg.TONotNullString() == "") return "";
                var tmpArg = this.arg.TONotNullString().Split(',').ToList();
                for (var i = tmpArg.Count() - 1; i >= 0; i--)
                {
                    if (tmpArg[i].Contains("PWD=")) tmpArg.RemoveAt(i);
                }
                return string.Join(",", tmpArg);
            }
        }
    }

    /// <summary>
    /// 匯出每月登入異常日誌
    /// </summary>
    public class C105MGrid2Model : loginlog
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string usr_name { get; set; }
        /// <summary>
        /// 單位
        /// </summary>
        public string unit_name { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string grp_name { get; set; }

        /// <summary>
        /// 靜態查表字典
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> LogTypeMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "LOGINFAIL", "登入失敗" },
            { "LOGIN", "登入" },
            { "LOGOUT", "登出" },
            { "CREATE", "新增" },
            { "UPDATE", "修改" },
            { "DELETE", "刪除" },
            { "UPLOAD", "上傳" },
            { "DOWNLOAD", "下載" },
            { "PRINT", "匯出報表" }
        };

        /// <summary>
        /// 登入訊息
        /// </summary>
        [NotDBField]
        public string logtype_Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.logtype)) return string.Empty; // 直接回傳
                string result = this.logtype;
                string timeSuffix = string.Empty; // 用於儲存時間標記的中文
                if (result.IndexOf("BEFORE") > -1) { result = result.Replace("BEFORE", ""); timeSuffix = "前"; }
                else if (result.IndexOf("AFTER") > -1) { result = result.Replace("AFTER", ""); timeSuffix = "後"; }
                string chineseOperation = "";
                if (LogTypeMappings.TryGetValue(result, out chineseOperation))
                {
                    result = chineseOperation;
                    return $"{chineseOperation}{timeSuffix}";
                }
                return $"{result}{timeSuffix}";
            }
        }
        
    }

}