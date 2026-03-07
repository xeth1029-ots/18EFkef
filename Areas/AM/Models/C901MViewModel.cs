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
using WKEFSERVICE.DataLayers;

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C901MViewModel
    {
        public C901MViewModel() { this.Form = new C901MFormModel(); }

        public C901MFormModel Form { get; set; }
    }

    public class C901MFormModel : PagingResultsViewModel
    {
        [Control(Mode = Control.Hidden)]
        [Display(Name = "年度")]
        public string Year { get; set; }

        public IList<SelectListItem> Year_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全顯示", Value = "" };
                var list = model.ReportRecordYears_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "教師")]
        public string Teacher_Name { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "首次上傳日期(起)")]
        public string FirstTime_S { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string FirstTime_S_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstTime_S))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(FirstTime_S, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                FirstTime_S = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "首次上傳日期(迄)")]
        public string FirstTime_E { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string FirstTime_E_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstTime_E))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(FirstTime_E, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                FirstTime_E = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "審核狀態")]
        public string AuditStatus { get; set; }

        public IList<SelectListItem> AuditStatus_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全顯示", Value = "" };
                var list = model.ReportRecordAuditStatus_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        public string UnitCode { get; set; }

        public IList<C901MGridModel> Grid { get; set; }
    }

    public class C901MGridModel : ReportRecord
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string AuditStatus_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpStr = "";
                var tmp = model.ReportRecordAuditStatus_List().Where(x => x.Value == this.AuditStatus.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        [NotDBField]
        public string FirstTime_Name
        {
            get
            {
                if (FirstTime.TONotNullString() == "") return "";
                string[] tmpList = FirstTime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }

        [NotDBField]
        public string AuditDatetime_Name
        {
            get
            {
                if (AuditDatetime.TONotNullString() == "") return "";
                string[] tmpList = AuditDatetime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }

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

        [NotDBField]
        public string TeacherName { get; set; }

        [NotDBField]
        public string TeacherEName { get; set; }
    }
}