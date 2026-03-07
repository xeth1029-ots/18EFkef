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
using System.Web;

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C107MViewModel
    {
        public C107MViewModel() { this.Form = new C107MFormModel(); }

        public C107MFormModel Form { get; set; }

        public C107MDetailModel Detail { get; set; }
    }

    public class C107MFormModel : PagingResultsViewModel
    {
        [Control(Mode = Control.Hidden)]
        [Display(Name = "關鍵字")]
        public string KeyWord { get; set; }

        public string DateS_A { get; set; }

        [Display(Name = "公告日期起")]
        [Control(Mode = Control.DatePicker)]
        public string DateS_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(DateS_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateS_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string DateS_B { get; set; }

        [Display(Name = "公告日期迄")]
        [Control(Mode = Control.DatePicker)]
        public string DateS_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(DateS_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateS_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string UpdatedDatetime_A { get; set; }

        [Display(Name = "異動日期起")]
        [Control(Mode = Control.DatePicker)]
        public string UpdatedDatetime_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(UpdatedDatetime_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                UpdatedDatetime_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string UpdatedDatetime_B { get; set; }

        [Display(Name = "異動日期迄")]
        [Control(Mode = Control.DatePicker)]
        public string UpdatedDatetime_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(UpdatedDatetime_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                UpdatedDatetime_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string DateE_A { get; set; }

        [Display(Name = "停用日期起")]
        [Control(Mode = Control.DatePicker)]
        public string DateE_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(DateE_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateE_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string DateE_B { get; set; }

        [Display(Name = "停用日期迄")]
        [Control(Mode = Control.DatePicker)]
        public string DateE_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(DateE_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateE_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "優先排序")]
        public string OrderField { get; set; }

        public IList<SelectListItem> OrderField_list
        {
            get
            {
                var dictionary = new Dictionary<string, string> {
                    { "a.UpdatedDatetime", "異動日期" },
                    { "a.DateS",           "公告日期" },
                };
                return MyCommonUtil.ConvertSelItems(dictionary);
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "優先排序")]
        public string OrderField_ASC_DESC { get; set; }
        public IList<SelectListItem> OrderField_ASC_DESC_list
        {
            get
            {
                var dictionary = new Dictionary<string, string> {
                    { "ASC",  "正排序" },
                    { "DESC", "倒排序" },
                };
                return MyCommonUtil.ConvertSelItems(dictionary);
            }
        }

        [Control(Mode = Control.Hidden)]
        public string Seq_forDel { get; set; }

        public IList<C107MGridModel> Grid { get; set; }
    }

    public class C107MGridModel : Notices
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string DateS_Name
        {
            get
            {
                //if (DateS.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(DateS) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (DateS.TONotNullString() == "") return "";
                string[] tmpList = DateS.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string DateE_Name
        {
            get
            {
                //if (DateE.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(DateE) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (DateE.TONotNullString() == "") return "";
                string[] tmpList = DateE.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string UpdatedDatetime_Name
        {
            get
            {
                //if (UpdatedDatetime.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(UpdatedDatetime) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (UpdatedDatetime.TONotNullString() == "") return "";
                string[] tmpList = UpdatedDatetime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }
    }

    public class C107MDetailModel : Notices
    {
        public C107MDetailModel() { this.IsNew = false; }

        /// <summary>Detail必要控件(Hidden)</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public bool IsNew { get; set; }

        [Required]
        [Display(Name = "標題")]
        public string Caption { get; set; }

        public string DateS { get; set; }

        [Required]
        [Display(Name = "公告日期")]
        [Control(Mode = Control.DatePicker)]
        public string DateS_AD
        {
            get
            {
                if (DateS.TONotNullString() == "") return "";
                string[] tmpList = DateS.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateS = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string DateE { get; set; }

        [Required]
        [Display(Name = "停用日期")]
        [Control(Mode = Control.DatePicker)]
        public string DateE_AD
        {
            get
            {
                if (DateE.TONotNullString() == "") return "";
                string[] tmpList = DateE.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Required]
        [Display(Name = "置頂")]
        public bool Is_OnTop
        {
            get { return IsOnTop == "1"; }
            set { IsOnTop = value ? "1" : "0"; }
        }

        /// <summary>檔案上傳 - 附件檔案資料表 (dbo.NoticesAttached)</summary>
        public IList<C107MAttachedsModel> Attacheds { get; set; }

        /// <summary>檔案上傳 - 新增附件檔案用元件</summary>
        [NotDBField]
        public HttpPostedFileBase _UPLOAD_FILE { get; set; }
    }

    public class C107MAttachedsModel : NoticesAttached
    {
        public C107MAttachedsModel()
        {
            this.isActive = true;
        }

        [NotDBField]
        public bool isActive { get; set; }

        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string UpdatedDatetime_Name
        {
            get
            {
                if (UpdatedDatetime.TONotNullString() == "") return "";
                string[] tmpList = UpdatedDatetime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }
    }
}