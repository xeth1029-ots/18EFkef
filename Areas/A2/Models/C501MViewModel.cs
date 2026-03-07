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
    public class C501MViewModel
    {
        public C501MViewModel() { this.Form = new C501MFormModel(); }

        public C501MFormModel Form { get; set; }

        public C501MDetailModel Detail { get; set; }
    }

    public class C501MFormModel : PagingResultsViewModel
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

        public IList<C501MGridModel> Grid { get; set; }
    }

    public class C501MGridModel : ReportRecord
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

        //[NotDBField]
        //public string ReportType_Name
        //{
        //    get
        //    {
        //        ShareCodeListModel model = new ShareCodeListModel();
        //        var tmpStr = "";
        //        var tmp = model.ReportRecordReportType_List().Where(x => x.Value == this.ReportType.TONotNullString());
        //        if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
        //        return tmpStr;
        //    }
        //}

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
    }

    public class C501MDetailModel : ReportRecord
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
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.JobAbilityCode_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Required]
        [Display(Name = "報告名稱")]
        public string ReportName { get; set; }

        [Required]
        [Display(Name = "簡要說明")]
        public string FileNameRemark { get; set; }

        /// <summary>檔案上傳 - 新增附件檔案用元件</summary>
        [Required]
        [NotDBField]
        [Display(Name = "檔案上傳")]
        public HttpPostedFileBase _UPLOAD_FILE { get; set; }
    }
}