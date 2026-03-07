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
    public class C402MViewModel
    {
        public C402MViewModel() { this.Form = new C402MFormModel(); }

        /// <summary>瀏覽頁</summary>
        public C402MFormModel Form { get; set; }

        /// <summary>編輯頁 - 出席管理</summary>
        public C402MDetailModel Detail { get; set; }
    }

    public class C402MFormModel : PagingResultsViewModel
    {

        public string Seq { get; set; }

        /// <summary>顯示頁數由1開始</summary>
        public string vPage { get; set; }

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

        public IList<C402MGridModel> Grid { get; set; }
    }

    public class C402MGridModel : Meeting
    {
        [NotDBField]
        public long? ROWID { get; set; }

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
    }

    public class C402MDetailModel
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

        public IList<C402MDetailRowsModel> DetailRows { get; set; }
    }

    public class C402MDetailRowsModel : MeetingAttend
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string TeacherName { get; set; }

        [NotDBField]
        public string UnitName { get; set; }

        [NotDBField]
        public bool Attend_Bool
        {
            get { if (this.Attend.TONotNullString() == "Y") return true; else return false; }
            set { this.Attend = (value) ? "Y" : "N"; }
        }

        [NotDBField]
        public bool TestPassed_Bool
        {
            get { if (this.TestPassed.TONotNullString() == "Y") return true; else return false; }
            set { this.TestPassed = (value) ? "Y" : "N"; }
        }
    }
}