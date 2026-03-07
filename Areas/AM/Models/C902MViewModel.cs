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
    public class C902MViewModel
    {
        public C902MViewModel() { this.Form = new C902MFormModel(); }

        public C902MFormModel Form { get; set; }

        public C902MDetailModel Detail { get; set; }
    }

    public class C902MFormModel : PagingResultsViewModel
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
                list = list.Where(x => x.Value != "" && x.Value != " ").ToList();  // 分署應該看不到"未送出"的報告才對
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        public string UnitCode { get; set; }

        [Control(Mode = Control.Hidden)]
        public string Seq_forUnAudit { get; set; }

        public IList<C902MGridModel> Grid { get; set; }
    }

    public class C902MGridModel : SelfReport
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string Year_Name
        {
            get
            {
                int tmpYear = -1;
                return (!int.TryParse(this.Year, out tmpYear)) ? "" : (tmpYear - 1911).ToString();
            }
        }

        [NotDBField]
        public string TeacherName { get; set; }

        [NotDBField]
        public string FirstTime_Name
        {
            get
            {
                if (FirstTime.TONotNullString() == "") return "";
                string[] tmpList = FirstTime.ToString().Split(' ');
                if (tmpList.Length != 2) return "";
                DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                return string.Concat(tmp.AddYears(-1911).ToString("yyy/MM/dd"), " ", tmpList[1]);
            }
        }

        [NotDBField]
        public string UpdatedDatetime_Name
        {
            get
            {
                if (UpdatedDatetime.TONotNullString() == "") return "";
                string[] tmpList = UpdatedDatetime.ToString().Split(' ');
                if (tmpList.Length != 2) return "";
                DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                return  string.Concat(tmp.AddYears(-1911).ToString("yyy/MM/dd"), " ", tmpList[1]);
            }
        }

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
        public string AuditDatetime_Name
        {
            get
            {
                if (AuditDatetime.TONotNullString() == "") return "";
                string[] tmpList = AuditDatetime.ToString().Split(' ');
                if (tmpList.Length != 2) return "";
                DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                return string.Concat(tmp.AddYears(-1911).ToString("yyy/MM/dd"), " ", tmpList[1]);
            }
        }
    }

    public class C902MDetailModel : SelfReport
    {
        [NotDBField]
        [Display(Name = "年度")]
        public string YearName { get; set; }

        [NotDBField]
        public string TeacherName { get; set; }

        [NotDBField]
        [Display(Name = "繳交期限")]
        public string UploadDeadline { get; set; }

        [Display(Name = "職能類別")]
        public string JobAbilityName { get; set; }

        [Display(Name = "課程單元")]
        public string CourseUnitCode { get; set; }

        [NotDBField]
        public string[] CourseUnitCode_SHOW
        {
            get
            {
                return (this.CourseUnitCode != null) ? this.CourseUnitCode.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.CourseUnitCode = string.Join(",", value.ToList());
            }
        }

        [NotDBField]
        public IList<CheckBoxListItem> CourseUnitCode_SHOW_list
        {
            get
            {
                // 把型態從 List<SelectListItem>
                // 　　轉成 List<CheckBoxListItem>
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> ListOrg = model.TeachUnit_list();
                IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
                foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
                return ListNew;
            }
        }

        [Display(Name = "授課對象")]
        public string OBJECT_TYPE { get; set; }

        [NotDBField]
        public IList<SelectListItem> OBJECT_TYPE_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.OBJECT_TYPE_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "學員人數")]
        public int? StudNum { get; set; }

        [Display(Name = "教學目標")]
        public string TObject { get; set; }

        [Display(Name = "教學方法")]
        public string TMethod { get; set; }

        [Display(Name = "其他")]
        public string Other { get; set; }

        [Display(Name = "其他佐證文件說明1")]
        public string Other1Remark { get; set; }

        [Display(Name = "其他佐證文件說明2")]
        public string Other2Remark { get; set; }

        [Display(Name = "三.成效指標達成度分析與檢討")]
        public string Effect { get; set; }

        [Display(Name = "(一) 教學計劃之改善(優化)")]
        public string PlanImprove { get; set; }

        [Display(Name = "(二) 評量設計之改善(優化)")]
        public string ExamImprove { get; set; }

        [Display(Name = "(三) 教材之改善(優化)")]
        public string materialsImprove { get; set; }

        public string Other3 { get; set; }

        /// <summary>審核狀態 (原狀態)</summary>
        [Display(Name = "目前狀態")]
        public string AuditStatus { get; set; }

        /// <summary>退回原因說明</summary>
        [Display(Name = "退回原因說明")]
        public string AuditReason { get; set; }

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
        [Required]
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

        [Display(Name = "審核時間")]
        public string AuditDatetime { get; set; }

        [Display(Name = "異動者")]
        public string UpdatedName { get; set; }

        [Display(Name = "異動時間")]
        public string UpdatedDatetime { get; set; }
    }
}