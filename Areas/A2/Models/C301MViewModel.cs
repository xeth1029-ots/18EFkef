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

namespace WKEFSERVICE.Areas.A2.Models
{
    public class C301MViewModel
    {
        public C301MViewModel() { this.Form = new C301MFormModel(); }

        public C301MFormModel Form { get; set; }

        public C301MDetailModel Detail { get; set; }
    }

    public class C301MFormModel : PagingResultsViewModel
    {
        public C301MFormModel() { this.IsSignUpStop = "1"; }

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

        [Control(Mode = Control.Hidden)]
        public string CreatedUnit_forSQL
        {
            get
            {
                SessionModel sm = SessionModel.Get();
                if (this.MeetingType.TONotNullString() == "1")
                {
                    // 【會議類別】：回流訓(署舉辦)，所有老師都可查的到
                    return null;
                }
                else
                if (this.MeetingType.TONotNullString() == "2")
                {
                    // 【會議類別】：共識會議(分署舉辦)，老師只能查到所屬分署舉辦的共識會議
                    // 也就是只能查到【舉辦單位】：發展署+教師所屬分署 的會議資料
                    return " AND a.CreatedUnit IN ('0', '" + sm.UserInfo.User.UNITID + "') ";
                }
                else
                {
                    // 列出：所有的回流訓 或 共識會議(發展署+教師所屬分署) 的會議資料
                    return
                        " AND ( " +
                            "( a.MeetingType='1' ) OR " +
                            "( a.MeetingType='2' AND a.CreatedUnit IN ('0', '" + sm.UserInfo.User.UNITID + "') ) " +
                        ") ";
                }
            }
        }

        //[Display(Name = "舉辦單位")]
        //public string CreatedUnit { get; set; }

        //public string[] CreatedUnit_SHOW
        //{
        //    get
        //    {
        //        if (this.CreatedUnit != null)
        //            return this.CreatedUnit.Replace("'", "").Split(',');
        //        else
        //            return new string[0];
        //    }
        //    set
        //    {
        //        if (value != null)
        //            this.CreatedUnit = string.Join(",", value.ToList());
        //    }
        //}

        //public IList<CheckBoxListItem> CreatedUnit_SHOW_list
        //{
        //    get
        //    {
        //        // 把型態從 List<SelectListItem>
        //        // 　　轉成 List<CheckBoxListItem>
        //        ShareCodeListModel model = new ShareCodeListModel();
        //        IList<SelectListItem> ListOrg = model.UNIT_All_List;
        //        IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
        //        foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
        //        return ListNew;
        //    }
        //}

        //[Display(Name = "會議地點")]
        //public string MeetLocated { get; set; }
        //
        //public string[] MeetLocated_SHOW
        //{
        //    get
        //    {
        //        if (this.MeetLocated != null)
        //            return this.MeetLocated.Replace("'", "").Split(',');
        //        else
        //            return new string[0];
        //    }
        //    set
        //    {
        //        if (value != null)
        //            this.MeetLocated = string.Join(",", value.ToList());
        //    }
        //}
        //
        //public IList<CheckBoxListItem> MeetLocated_SHOW_list
        //{
        //    get
        //    {
        //        // 把型態從 List<SelectListItem>
        //        // 　　轉成 List<CheckBoxListItem>
        //        ShareCodeListModel model = new ShareCodeListModel();
        //        IList<SelectListItem> ListOrg = model.MeetLocated_List(true);
        //        IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
        //        foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
        //        return ListNew;
        //    }
        //}

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

        [Control(Mode = Control.Hidden)]
        public string LoginAccount { get; set; }

        public IList<C301MGridModel> Grid { get; set; }
    }

    public class C301MGridModel : Meeting
    {
        [NotDBField]
        public long? ROWID { get; set; }

        /// <summary>是否已報名</summary>
        [NotDBField]
        public int isSignUp { get; set; }

        /// <summary>是否可以報名</summary>
        [NotDBField]
        public int isCanSignUp { get; set; }

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

    public class C301MDetailModel
    {
        public Int64? Seq { get; set; }

        /// <summary>是否已報名</summary>
        [NotDBField]
        public int isSignUp { get; set; }

        /// <summary>是否可以報名</summary>
        [NotDBField]
        public int isCanSignUp { get; set; }

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

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "天數")]
        public int? Days { get; set; }

        [Display(Name = "限制人數")]
        public int? MaximumPeople { get; set; }

        [Display(Name = "舉辦單位")]
        public string UnitName { get; set; }

        [Display(Name = "會議說明")]
        public string Remark { get; set; }

        [Display(Name = "附圖")]
        public string PicName { get; set; }

        public IList<C301MAttachedsModel> Attacheds { get; set; }
    }

    public class C301MAttachedsModel : MeetingAttached
    {
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