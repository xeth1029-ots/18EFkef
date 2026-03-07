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

namespace WKEFSERVICE.Areas.A3.Models
{
    public class C202MViewModel
    {
        public C202MViewModel() { this.Form = new C202MFormModel(); }

        public C202MFormModel Form { get; set; }

        public C202MDetailModel Detail { get; set; }
    }

    public class C202MFormModel : PagingResultsViewModel
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
                list = list.Where(x => x.Value != " ").ToList();  // 分署應該看不到"未送出"的報告才對
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        public string UnitCode { get; set; }

        public IList<C202MGridModel> Grid { get; set; }
    }

    public class C202MGridModel : ReportRecord
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

    public class C202MDetailModel : ReportRecord
    {
        [Display(Name = "年度")]
        public string YearName { get; set; }

        [Display(Name = "教師")]
        public string TeacherName { get; set; }

        public string TeacherEName { get; set; }

        public string UnitCode { get; set; }

        [Display(Name = "職能類別")]
        public string JobAbilityName { get; set; }

        [Display(Name = "報告名稱")]
        public string ReportName { get; set; }

        [Display(Name = "簡要說明")]
        public string FileNameRemark { get; set; }

        [Display(Name = "首次上傳時間")]
        public string FirstTime { get; set; }

        [NotDBField]
        public string FirstTimeName
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

        [Display(Name = "繳交期限")]
        public string UploadDeadline
        {
            get
            {
                A2DAO dao = new A2DAO();
                return dao.GetUploadDeadlineStr(this.Year, this.UnitCode);
            }
        }

        [Display(Name = "檔案名稱")]
        public string FileNameOrg { get; set; }

        /// <summary>審核狀態 (原狀態)</summary>
        [Display(Name = "目前狀態")]
        public string AuditStatus { get; set; }

        /// <summary>審核狀態 (原狀態)</summary>
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

        /// <summary>審核狀態 使用者審核動作用</summary>
        [Display(Name = "審核狀態")]
        public string AuditStatus_New { get; set; }

        [NotDBField]
        public IList<SelectListItem> AuditStatus_New_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.ReportRecordAuditStatus_forAudit_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "審核者")]
        public string AuditName { get; set; }

        [Display(Name = "異動者")]
        public string UpdatedName { get; set; }

        [Display(Name = "審核時間")]
        public string AuditDatetime { get; set; }

        [Display(Name = "異動時間")]
        public string UpdatedDatetime { get; set; }
    }
}