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
    public class C302MViewModel
    {
        public C302MViewModel() { this.Form = new C302MFormModel(); }

        public C302MFormModel Form { get; set; }

        public C302MDetailModel Detail { get; set; }
    }

    public class C302MFormModel : PagingResultsViewModel
    {
        /// <summary>登入者帳號</summary>
        public string LoginAccount { get; set; }

        /// <summary>取消報名用暫存</summary>
        public long Seq_forUnSign { get; set; }

        public IList<C302MGridModel> Grid { get; set; }
    }

    public class C302MGridModel
    {
        [NotDBField]
        public long? ROWID { get; set; }

        public long Seq { get; set; }

        public string MeetingName { get; set; }

        public string CreatedUnit { get; set; }

        public string UnitName { get; set; }

        public string isAttend { get; set; }

        public string isTestPassed { get; set; }

        public int TestScore { get; set; }

        /// <summary>實際出席狀況</summary>
        public string isAttend_Name { get { return (this.isAttend == "Y") ? "有出席" : "未出席"; } }

        /// <summary>是否通過測驗</summary>
        public string isTestPassed_Name
        {
            get
            {
                if (this.isTestPassed.TONotNullString() == "") return "";
                if (this.isTestPassed.TONotNullString() == "Y") return string.Concat("通過", " (成績：", this.TestScore, ")");
                else return "未通過";
            }
        }

        public string MeetingDateS { get; set; }

        public string MeetingDateE { get; set; }

        public string TimeS { get; set; }

        public string TimeE { get; set; }

        public string SignUpDateS { get; set; }

        public string SignUpDateE { get; set; }

        public string SignUpDatetime { get; set; }

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

        public string SignUpDateName
        {
            get
            {
                if (SignUpDateS.TONotNullString() == "" || SignUpDateE.TONotNullString() == "") return "";
                string[] tmpDateListS = SignUpDateS.TONotNullString().Split(' ');
                string[] tmpDateListE = SignUpDateE.TONotNullString().Split(' ');
                DateTime DateS = HelperUtil.TransToDateTime(tmpDateListS[0], "-") ?? DateTime.MinValue;
                DateTime DateE = HelperUtil.TransToDateTime(tmpDateListE[0], "-") ?? DateTime.MinValue;
                string[] tmpTimeListS = tmpDateListS[1].TONotNullString().Split(':');
                string[] tmpTimeListE = tmpDateListE[1].TONotNullString().Split(':');
                string tmpTimeS = string.Concat(tmpTimeListS[0], ":", tmpTimeListS[1]);
                string tmpTimeE = string.Concat(tmpTimeListE[0], ":", tmpTimeListE[1]);
                DateS = DateS.AddYears(-1911);
                DateE = DateE.AddYears(-1911);
                return string.Concat(DateS.ToString("yyy/MM/dd"), " ", tmpTimeS, " ~ ", DateE.ToString("yyy/MM/dd"), " ", tmpTimeE);
            }
        }

        public string MeetingType { get; set; }

        public string MeetingTypeName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.MeetingType_list().Where(x => x.Value == this.MeetingType.TONotNullString());
                var tmpStr = (tmp.Any()) ? tmp.FirstOrDefault().Text : "";
                return tmpStr;
            }
        }

        public string Location { get; set; }

        public string SignUpStr { get; set; }
    }

    public class C302MDetailModel
    {
        public Int64? Seq { get; set; }

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
                var tmp = model.MeetingType_list().Where(x => x.Value == this.MeetingType.TONotNullString());
                var tmpStr = (tmp.Any()) ? tmp.FirstOrDefault().Text : "";
                return tmpStr;
            }
        }

        [Display(Name = "會議名稱")]
        public string MeetingName { get; set; }

        [Display(Name = "地點")]
        public string Location { get; set; }

        [Display(Name = "限制人數")]
        public int? MaximumPeople { get; set; }

        [Display(Name = "舉辦單位")]
        public string UnitName { get; set; }

        [Display(Name = "會議說明")]
        public string Remark { get; set; }

        [Display(Name = "附圖")]
        public string PicName { get; set; }

        public IList<C302MAttachedsModel> Attacheds { get; set; }
    }

    public class C302MAttachedsModel : MeetingAttached
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
                return string.Concat(tmp.AddYears(-1911).ToString("yyy/MM/dd"), " ", tmpTime);
            }
        }
    }
}