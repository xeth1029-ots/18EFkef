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

namespace WKEFSERVICE.Areas.A2.Models
{
    public class C502MViewModel
    {
        public C502MViewModel() { this.Form = new C502MFormModel(); }

        public C502MFormModel Form { get; set; }

        public C502MDetailModel Detail { get; set; }
    }

    public class C502MFormModel : PagingResultsViewModel
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
        public string TeacherAccount { get; set; }

        [Control(Mode = Control.Hidden)]
        public string Seq_forDel { get; set; }

        public IList<C502MGridModel> Grid { get; set; }
    }

    public class C502MGridModel : SelfReport
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string Year_Name
        {
            get
            {
                int tmpYear = -1;
                if (!int.TryParse(this.Year, out tmpYear)) return "";
                return (tmpYear - 1911).ToString();
            }
        }

        [NotDBField]
        [Display(Name = "審核狀態")]
        public string AuditStatus_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.ReportRecordAuditStatus_List().Where(x => x.Value == this.AuditStatus.TONotNullString());
                return tmp.Any() ? tmp.FirstOrDefault().Text : "";
            }
        }

        [NotDBField]
        public string FirstTime_Name
        {
            get
            {
                if (FirstTime.TONotNullString() == "") return "";
                string[] tmpList = FirstTime.ToString().Split(' ');
                if (tmpList.Length < 2) return "";
                DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                return string.Concat(tmp.AddYears(-1911).ToString("yyy/MM/dd"), " ", tmpList[1].Substring(0, 5));
            }
        }

        [NotDBField]
        public string UpdatedDatetime_Name
        {
            get
            {
                if (UpdatedDatetime.TONotNullString() == "") return "";
                string[] tmpList = UpdatedDatetime.ToString().Split(' ');
                if (tmpList.Length < 2) return "";
                DateTime tmp = HelperUtil.TransToDateTime(tmpList[0], "-") ?? DateTime.MinValue;
                return string.Concat(tmp.AddYears(-1911).ToString("yyy/MM/dd"), " ", tmpList[1].Substring(0, 5));
            }
        }
    }

    public class C502MDetailModel : SelfReport
    {
        /// <summary>Detail必要控件(Hidden)</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public bool IsNew { get; set; }

        [Required]
        [Display(Name = "年度")]
        public string Year { get; set; }

        [NotDBField]
        public IList<SelectListItem> Year_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.ReportRecordYears_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [NotDBField]
        [Display(Name = "繳交期限")]
        public string UploadDeadline { get; set; }

        [Required]
        [Display(Name = "職能類別")]
        public string JobAbilityCode { get; set; }

        [NotDBField]
        public IList<SelectListItem> JobAbilityCode_list
        {
            get
            {
                SessionModel sm = SessionModel.Get();
                var UserData = new A2DAO().GetRow(new Teacher() { ACCOUNT = sm.UserInfo.UserNo });
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.JobAbilityCode_List();
                if (UserData != null)
                {
                    if (UserData.TeachJobAbilityDC.TONotNullString() != "Y") list = list.Where(x => x.Value != "DC").ToList();
                    if (UserData.TeachJobAbilityBC.TONotNullString() != "Y") list = list.Where(x => x.Value != "BC").ToList();
                    if (UserData.TeachJobAbilityKC.TONotNullString() != "Y") list = list.Where(x => x.Value != "KC").ToList();
                }
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Required]
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

        [Required]
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

        [Required]
        [Display(Name = "學員人數")]
        public int? StudNum { get; set; }

        [Required]
        [Display(Name = "教學目標")]
        public string TObject { get; set; }

        [Required]
        [Display(Name = "教學方法")]
        public string TMethod { get; set; }

        //[Required]  由程式檢查
        [NotDBField]
        [Display(Name = "教材簡報")]
        public HttpPostedFileBase FILE_Tslides { get; set; }
        public string TslidesOrg { get; set; }
        public string TslidesNew { get; set; }
        public string TslidesType { get; set; }

        [Display(Name = "其他")]
        public string Other { get; set; }

        //[Required]  由程式檢查
        [NotDBField]
        [Display(Name = "學習分組報告")]
        public HttpPostedFileBase FILE_StudyGReport { get; set; }
        public string StudyGReportOrg { get; set; }
        public string StudyGReportNew { get; set; }
        public string StudyGReportType { get; set; }

        //[Required]  由程式檢查
        [NotDBField]
        [Display(Name = "學習單")]
        public HttpPostedFileBase FILE_Worksheets { get; set; }
        public string WorksheetsOrg { get; set; }
        public string WorksheetsNew { get; set; }
        public string WorksheetsType { get; set; }

        //[Required]  由程式檢查
        [NotDBField]
        [Display(Name = "作業")]
        public HttpPostedFileBase FILE_HomeWork { get; set; }
        public string HomeWorkOrg { get; set; }
        public string HomeWorkNew { get; set; }
        public string HomeWorkType { get; set; }

        //[Required]  由程式檢查
        [NotDBField]
        [Display(Name = "心得報告")]
        public HttpPostedFileBase FILE_ExpReport { get; set; }
        public string ExpReportOrg { get; set; }
        public string ExpReportNew { get; set; }
        public string ExpReportType { get; set; }

        //[Required]  由程式檢查
        [NotDBField]
        [Display(Name = "測驗")]
        public HttpPostedFileBase FILE_Exam { get; set; }
        public string ExamOrg { get; set; }
        public string ExamNew { get; set; }
        public string ExamType { get; set; }

        [NotDBField]
        [Display(Name = "其他佐證文件1")]
        public HttpPostedFileBase FILE_Other1 { get; set; }
        public string Other1Org { get; set; }
        public string Other1New { get; set; }
        public string Other1Type { get; set; }
        [Display(Name = "其他佐證文件說明1")]
        public string Other1Remark { get; set; }

        [NotDBField]
        [Display(Name = "其他佐證文件2")]
        public HttpPostedFileBase FILE_Other2 { get; set; }
        public string Other2Org { get; set; }
        public string Other2New { get; set; }
        public string Other2Type { get; set; }
        [Display(Name = "其他佐證文件說明2")]
        public string Other2Remark { get; set; }

        /// <summary>三.成效指標達成度分析與檢討</summary>
        [Required]
        [Display(Name = "三.成效指標達成度分析與檢討")]
        public string Effect { get; set; }

        [Required]
        [Display(Name = "(一) 教學計劃之改善(優化)")]
        public string PlanImprove { get; set; }

        [Required]
        [Display(Name = "(二) 評量設計之改善(優化)")]
        public string ExamImprove { get; set; }

        [Required]
        [Display(Name = "(三) 教材之改善(優化)")]
        public string materialsImprove { get; set; }

        /// <summary>五.其他</summary>
        public string Other3 { get; set; }

        /// <summary>審核狀態</summary>
        [NotDBField]
        [Display(Name = "審核狀態")]
        public string AuditStatus_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.ReportRecordAuditStatus_List().Where(x => x.Value == this.AuditStatus.TONotNullString());
                return tmp.Any() ? tmp.FirstOrDefault().Text : "(查無資料)";
            }
        }

        /// <summary>退回原因說明</summary>
        [NotDBField]
        [Display(Name = "退回原因說明")]
        public string AuditReason_Txt { get { return AuditReason; } }
    }
}