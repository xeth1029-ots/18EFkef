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

namespace WKEFSERVICE.Areas.A3.Models
{
    public class C401MViewModel
    {
        public C401MViewModel() { this.Form = new C401MFormModel(); }

        /// <summary>瀏覽頁</summary>
        public C401MFormModel Form { get; set; }

        /// <summary>瀏覽頁 -> 按鈕 修改</summary>
        public C401MDetailModel Detail { get; set; }

        /// <summary>瀏覽頁 -> 按鈕 報名管理</summary>
        public C401MDetailSignUpModel DetailSignUp { get; set; }
    }

    public class C401MFormModel : PagingResultsViewModel
    {
        public C401MFormModel() { this.IsSignUpStop = "1"; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "會議類別")]
        public string MeetingType { get; set; }

        public IList<SelectListItem> MeetingType_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "不區分", Value = "" };
                var list = model.MeetingType_list();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "會議名稱")]
        public string MeetingName { get; set; }

        [NotDBField]
        [Control(Mode = Control.Hidden)]
        [Display(Name = "會議日期起")]
        public string MeetingDateS { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string MeetingDateS_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(MeetingDateS, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                MeetingDateS = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [NotDBField]
        [Control(Mode = Control.Hidden)]
        [Display(Name = "會議日期迄")]
        public string MeetingDateE { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string MeetingDateE_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(MeetingDateE, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                MeetingDateE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [NotDBField]
        [Control(Mode = Control.Hidden)]
        [Display(Name = "報名日期起")]
        public string SignUpDateS { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string SignUpDateS_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(SignUpDateS, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                SignUpDateS = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [NotDBField]
        [Control(Mode = Control.Hidden)]
        [Display(Name = "報名日期迄")]
        public string SignUpDateE { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string SignUpDateE_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(SignUpDateE, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                SignUpDateE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Display(Name = "舉辦單位")]
        public string CreatedUnit { get; set; }

        public string[] CreatedUnit_SHOW
        {
            get
            {
                return (this.CreatedUnit != null) ? this.CreatedUnit.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.CreatedUnit = string.Join(",", value.ToList());
            }
        }

        public IList<CheckBoxListItem> CreatedUnit_SHOW_list
        {
            get
            {
                // 把型態從 List<SelectListItem>
                // 　　轉成 List<CheckBoxListItem>
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> ListOrg = model.UNIT_List;
                IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
                foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
                return ListNew;
            }
        }

        [Display(Name = "會議地點")]
        public string MeetLocated { get; set; }

        public string[] MeetLocated_SHOW
        {
            get
            {
                return (this.MeetLocated != null) ? this.MeetLocated.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.MeetLocated = string.Join(",", value.ToList());
            }
        }

        public IList<CheckBoxListItem> MeetLocated_SHOW_list
        {
            get
            {
                // 把型態從 List<SelectListItem>
                // 　　轉成 List<CheckBoxListItem>
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> ListOrg = model.MeetLocated_List(true);
                IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
                foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
                return ListNew;
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "包含已截止報名會議")]
        public string IsSignUpStop { get; set; }

        [Control(Mode = Control.Hidden)]
        public string IsSignUpStop_forSQL
        {
            get
            {
                if (this.IsSignUpStop.TONotNullString() == "0") return " AND a.SignUpDateE>=GETDATE() ";
                return "";
            }
        }

        [Control(Mode = Control.Hidden)]
        public string Seq_forDel { get; set; }

        public IList<C401MGridModel> Grid { get; set; }
    }

    public class C401MGridModel : Meeting
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string MeetingDateName
        {
            get
            {
                bool isNullS = (this.MeetingDateS.TONotNullString() == "");
                bool isNullE = (this.MeetingDateE.TONotNullString() == "");
                DateTime DateS = HelperUtil.TransToDateTime(this.MeetingDateS, "/") ?? DateTime.MinValue;
                DateTime DateE = HelperUtil.TransToDateTime(this.MeetingDateE, "/") ?? DateTime.MinValue;
                if (!isNullS) DateS = DateS.AddYears(-1911);
                if (!isNullE) DateE = DateE.AddYears(-1911);
                string Result = "";
                if (!isNullS)
                {
                    Result = DateS.ToString("yyy/MM/dd");
                    if (!isNullE)
                    {
                        Result = Result + " ~ " + DateE.ToString("yyy/MM/dd");
                    }
                }
                return Result;
            }
        }

        [NotDBField]
        public string SignUpDateName
        {
            get
            {
                if (SignUpDateS.TONotNullString() == "") return "";
                if (SignUpDateE.TONotNullString() == "") return "";
                string[] tmpDateListS = SignUpDateS.TONotNullString().Split(' ');
                string[] tmpDateListE = SignUpDateE.TONotNullString().Split(' ');
                DateTime DateS = HelperUtil.TransToDateTime(tmpDateListS[0], "-") ?? DateTime.MinValue;
                DateTime DateE = HelperUtil.TransToDateTime(tmpDateListE[0], "-") ?? DateTime.MinValue;
                string[] tmpTimeListS = tmpDateListS[1].TONotNullString().Split(':');
                string[] tmpTimeListE = tmpDateListE[1].TONotNullString().Split(':');
                string tmpTimeS = tmpTimeListS[0] + ":" + tmpTimeListS[1];
                string tmpTimeE = tmpTimeListE[0] + ":" + tmpTimeListE[1];
                DateS = DateS.AddYears(-1911);
                DateE = DateE.AddYears(-1911);
                return
                    DateS.ToString("yyy/MM/dd") + " " + tmpTimeS + " ~ " +
                    DateE.ToString("yyy/MM/dd") + " " + tmpTimeE;
            }
        }

        [NotDBField]
        public string MeetingTypeName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpStr = "";
                var tmp = model.MeetingType_list().Where(x => x.Value == this.MeetingType.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }
    }

    public class C401MDetailModel : Meeting
    {
        /// <summary>Detail必要控件(Hidden)</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public bool IsNew { get; set; }

        [Required]
        [Display(Name = "類別")]
        public string MeetingType { get; set; }

        public IList<SelectListItem> MeetingType_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.MeetingType_list();
            }
        }

        [Required]
        [Display(Name = "會議名稱")]
        public string MeetingName { get; set; }

        [Required]
        [Display(Name = "日期(起)")]
        [Control(Mode = Control.DatePicker)]
        public string MeetingDateS_AD
        {
            get
            {
                if (MeetingDateS.TONotNullString() == "") return "";
                DateTime tmp = HelperUtil.TransToDateTime(MeetingDateS, "/") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                MeetingDateS = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Display(Name = "日期(迄)")]
        [Control(Mode = Control.DatePicker)]
        public string MeetingDateE_AD
        {
            get
            {
                if (MeetingDateE.TONotNullString() == "") return "";
                DateTime tmp = HelperUtil.TransToDateTime(MeetingDateE, "/") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                MeetingDateE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Required]
        [Display(Name = "時間(起)")]
        public string TimeS { get; set; }

        [Required]
        [Display(Name = "時間(迄)")]
        public string TimeE { get; set; }

        [Display(Name = "天數")]
        public int? Days { get; set; }

        [Display(Name = "地點")]
        public string Location { get; set; }

        [Display(Name = "限制人數")]
        public int? MaximumPeople { get; set; }

        [Required]
        [Display(Name = "報名日期(起)")]
        [Control(Mode = Control.DatePicker)]
        public string SignUpDateS_AD
        {
            get
            {
                if (SignUpDateS.TONotNullString() == "") return "";
                string[] tmpList = SignUpDateS.TONotNullString().Split(' ');
                if (tmpList.ToCount() >= 1)
                {
                    DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                    return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
                }
                else return "";
            }
            set
            {
                string[] tmpList = value.TONotNullString().Split(' ');
                string tmpTime = "00:00";
                if (tmpList.ToCount() >= 2) tmpTime = tmpList[1];
                SignUpDateS = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-") + " " + tmpTime;
            }
        }

        [Required]
        [Display(Name = "報名日期(迄)")]
        [Control(Mode = Control.DatePicker)]
        public string SignUpDateE_AD
        {
            get
            {
                if (SignUpDateE.TONotNullString() == "") return "";
                string[] tmpList = SignUpDateE.TONotNullString().Split(' ');
                if (tmpList.ToCount() >= 1)
                {
                    DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                    return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
                }
                else return "";
            }
            set
            {
                string[] tmpList = value.TONotNullString().Split(' ');
                string tmpTime = "00:00";
                if (tmpList.ToCount() >= 2) tmpTime = tmpList[1];
                SignUpDateE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-") + " " + tmpTime;
            }
        }

        [Required]
        [Display(Name = "報名時間(起)")]
        public string SignUpDateS_Time
        {
            get
            {
                if (SignUpDateS.TONotNullString() == "") return "";
                string[] tmpList = SignUpDateS.TONotNullString().Split(' ');
                if (tmpList.ToCount() >= 1) return tmpList[1];
                else return "";
            }
            set
            {
                string[] tmpList = SignUpDateS.TONotNullString().Split(' ');
                string tmpDate = "";
                if (tmpList.ToCount() >= 2) tmpDate = tmpList[0];
                SignUpDateS = tmpDate + " " + value;
            }
        }

        [Required]
        [Display(Name = "報名時間(迄)")]
        public string SignUpDateE_Time
        {
            get
            {
                if (SignUpDateE.TONotNullString() == "") return "";
                string[] tmpList = SignUpDateE.TONotNullString().Split(' ');
                if (tmpList.ToCount() >= 1) return tmpList[1];
                else return "";
            }
            set
            {
                string[] tmpList = SignUpDateE.TONotNullString().Split(' ');
                string tmpDate = "";
                if (tmpList.ToCount() >= 2) tmpDate = tmpList[0];
                SignUpDateE = tmpDate + " " + value;
            }
        }

        [Required]
        [Display(Name = "上架日期")]
        [Control(Mode = Control.DatePicker)]
        public string MeetingRelease_AD
        {
            get
            {
                if (MeetingRelease.TONotNullString() == "") return "";
                string[] tmpList = MeetingRelease.TONotNullString().Split(' ');
                if (tmpList.ToCount() >= 1)
                {
                    DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                    return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
                }
                else return "";
            }
            set
            {
                string[] tmpList = value.TONotNullString().Split(' ');
                string tmpTime = "00:00";
                if (tmpList.ToCount() >= 2) tmpTime = tmpList[1];
                MeetingRelease = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-") + " " + tmpTime;
            }
        }

        [Required]
        [Display(Name = "上架時間")]
        public string MeetingRelease_Time
        {
            get
            {
                if (MeetingRelease.TONotNullString() == "") return "";
                string[] tmpList = MeetingRelease.TONotNullString().Split(' ');
                if (tmpList.ToCount() >= 1) return tmpList[1];
                else return "";
            }
            set
            {
                string[] tmpList = MeetingRelease.TONotNullString().Split(' ');
                string tmpDate = "";
                if (tmpList.ToCount() >= 2) tmpDate = tmpList[0];
                MeetingRelease = tmpDate + " " + value;
            }
        }

        [Display(Name = "舉辦單位")]
        public string CreatedUnitName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                string tmpStr = "";
                var tmp = model.UNIT_All_List.Where(x => x.Value == this.CreatedUnit.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        [Display(Name = "會議說明")]
        public string Remark { get; set; }

        [Display(Name = "圖檔")]
        public string PicName { get; set; }

        [NotDBField]
        public HttpPostedFileBase PicName_FILE { get; set; }

        /// <summary>資料夾路徑名 (暫存，程式用)</summary>
        [NotDBField]
        public string _tmpPicPath { get; set; }

        [Display(Name = "圖檔提示文字")]
        public string PicHint { get; set; }

        [Display(Name = "圖檔位置")]
        public string PicAlign { get; set; }

        public IList<SelectListItem> PicAlign_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.PicAlign_list();
            }
        }

        /// <summary>檔案上傳 - 附件檔案資料表 (dbo.MeetingAttached)</summary>
        public IList<C401MAttachedsModel> Attacheds { get; set; }

        /// <summary>檔案上傳 - 新增附件檔案用元件</summary>
        [NotDBField]
        public HttpPostedFileBase _UPLOAD_FILE { get; set; }
    }

    public class C401MAttachedsModel : MeetingAttached
    {
        public C401MAttachedsModel() { this.isActive = true; }

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

    public class C401MDetailSignUpModel
    {
        public Int64? Seq { get; set; }
        
        /// <summary>顯示頁數由1開始</summary>
        public string vPage { get; set; }

        [Display(Name = "日期")]
        public string MeetingDate { get; set; }

        [Display(Name = "時間")]
        public string MeetingTime { get; set; }

        [Display(Name = "報名日期")]
        public string SignUpDateTime { get; set; }

        public string MeetingType { get; set; }

        [Display(Name = "類別")]
        public string MeetingTypeName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpStr = "";
                var tmp = model.MeetingType_list().Where(x => x.Value == this.MeetingType.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        [Display(Name = "會議名稱")]
        public string MeetingName { get; set; }

        [Display(Name = "地點")]
        public string Location { get; set; }

        [Display(Name = "限制人數")]
        public int? MaximumPeople { get; set; }

        public string MeetingDateS { get; set; }

        public string MeetingDateE { get; set; }

        public string TimeS { get; set; }

        public string TimeE { get; set; }

        public string CreatedUnit { get; set; }

        /// <summary>教師報名 清單 (資料庫)</summary>
        public IList<C401MDetailSignUpListModel> SignUpList { get; set; }

        /// <summary>教師報名 清單 (刪除功能暫存用)</summary>
        public string TempSignUpListDelete_TeacherAccount { get; set; }

        /// <summary>教師彈窗 搜尋 (暫存，程式用)</summary>
        public string TeacherSearch { get; set; }

        /// <summary>教師彈窗 清單 (暫存，程式用)</summary>
        public IList<C401MTeacherPopupListModel> TeacherSearchList { get; set; }
    }

    public class C401MDetailSignUpListModel : MeetingSignUp
    {
        public C401MDetailSignUpListModel() { this.isActive = true; }

        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public bool isActive { get; set; }

        [NotDBField]
        public string TeacherName { get; set; }

        [NotDBField]
        public string SignUpTypeName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.SignUpType_list().Where(x => x.Value == this.SignUpType).FirstOrDefault().Text;
            }
        }
    }

    public class C401MTeacherPopupListModel
    {
        public long? ROWID { get; set; }
        public bool IsChecked { get; set; }
        public string TeacherAccount { get; set; }
        public string TeacherName { get; set; }
    }
}